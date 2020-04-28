using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace BLB.CustomMessageBoxButtons {
    public class CustomMessageBox: DaggerfallMessageBox {

        const int minBoxWidth = 132;
        MultiFormatTextLabel label = new MultiFormatTextLabel();
        int customYPos = -1;
        string selectedCustomButton = "";
        List<Button> buttons = new List<Button>();
        Panel buttonPanel = new Panel();
        bool buttonClicked = false;
        int buttonSpacing = 32;
        int buttonTextDistance = 4;
        Panel messagePanel = new Panel();


        public CustomMessageBox(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null, bool wrapText = false, int posY = -1)
            : base(uiManager, previous)
        {
            if (wrapText)
            {
                label.WrapText = label.WrapWords = true;
                // If wrapping text, set maxWidth to 288. This is just an aesthetically chosen value, as
                // it is the widest text can be without making the parchment textures expand off the edges of the screen.
                label.MaxTextWidth = 288;
            }

            if (posY > -1)
                customYPos = posY;
        }

        public string SelectedCustomButton
        {
            get { return selectedCustomButton; }
        }

        
        public Button AddCustomButton(int messageBoxButton, string tag, bool defaultButton = false)
        {
            if (!IsSetup)
                Setup();

            // If this is to become default button, first unset any other default buttons
            // Only one button in collection can be default
            if (defaultButton)
            {
                foreach (Button b in buttons)
                    b.DefaultButton = false;
            }

            Texture2D background;// = DaggerfallUI.GetTextureFromCifRci(buttonsFilename, (int)messageBoxButton);
            TextureReplacement.TryImportImage("button" + tag, true, out background);
            //Vector2 size = TextureReplacement.GetSize(background, buttonsFilename, (int)messageBoxButton);
            Vector2 size = new Vector2(background.width, background.height);
            Button button = DaggerfallUI.AddButton(Vector2.zero, 
                size, buttonPanel);
            button.BackgroundTexture = background;
            button.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            button.Tag = tag;
            button.OnMouseClick += CustomButtonClickHandler;
            button.DefaultButton = defaultButton;
            //button.Hotkey = DaggerfallShortcut.GetBinding(ToShortcutButton(messageBoxButton));
            buttons.Add(button);

            // Once a button has been added the owner is expecting some kind of input from player
            // Don't allow a messagebox with buttons to be cancelled with escape
            AllowCancel = false;
            UpdatePanelSizes();

            return button;
        }

        void UpdatePanelSizes()
        {
            Vector2 finalSize = new Vector2();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Position = new Vector2(finalSize.x, 0);

                Vector2 buttonSize = buttons[i].Size;
                finalSize.x += buttonSize.x;

                if (buttonSize.y > finalSize.y)
                    finalSize.y = buttonSize.y;

                if (i < buttons.Count - 1)
                    finalSize.x += buttonSpacing;
            }

            // If buttons have been added, resize label text by adding in the height of the finalized button panel.
            if (finalSize.y - buttonPanel.Size.y > 0)
                label.ResizeY(label.Size.y + (finalSize.y - buttonPanel.Size.y) + buttonTextDistance);

            buttonPanel.Size = finalSize;

            // Position buttons to be buttonTextDistance pixels below the repositioned text
            if (buttons.Count > 0)
            {
                float buttonY = messagePanel.Size.y - ((messagePanel.Size.y - label.Size.y) / 2) - buttonPanel.Size.y - messagePanel.BottomMargin;
                buttonPanel.Position = new Vector2(buttonPanel.Position.x, buttonY);
            }

            // Resize the message panel to get a clean border of 22x22 pixel textures
            int minimum = 44;
            float width = label.Size.x + messagePanel.LeftMargin + messagePanel.RightMargin;
            float height = label.Size.y + messagePanel.TopMargin + messagePanel.BottomMargin;

            // Enforce a minimum size
            if (width < minBoxWidth)
                width = minBoxWidth;

            if (width > minimum)
                width = (float)Math.Ceiling(width / 22) * 22;
            else
                width = minimum;

            if (height > minimum)
                height = (float)Math.Ceiling(height / 22) * 22;
            else
                height = minimum;

            messagePanel.Size = new Vector2(width, height);
        }

        void CustomButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            buttonClicked = true;
            selectedCustomButton = (string)sender.Tag;
            RaiseOnCustomButtonClickEvent(this, selectedCustomButton);
        }

        public delegate void OnCustomButtonClickHandler(DaggerfallMessageBox sender, string messageBoxButton);
        public event OnCustomButtonClickHandler OnCustomButtonClick;
        void RaiseOnCustomButtonClickEvent(DaggerfallMessageBox sender, string messageBoxButton)
        {
            if (OnCustomButtonClick != null)
                OnCustomButtonClick(sender, messageBoxButton);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

    }
}