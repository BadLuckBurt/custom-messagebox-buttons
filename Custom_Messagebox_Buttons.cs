using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace BLB.CustomMessageboxButtons
{
    //this class initializes the mod.
    public class CustomMessageboxButtonsModLoader : MonoBehaviour
    {
        public static Mod mod;
		public static GameObject go;

        //like in the last example, this is used to setup the Mod.  This gets called at Start state.
        [Invoke]
        public static void InitAtStartState(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<CustomMessageboxButtonsModLoader>();

            Debug.Log("Started setup of : " + mod.Title);

            mod.IsReady = true;
        }

        [Invoke(StateManager.StateTypes.Game)]
        public static void InitAtGameState(InitParams initParams)
        {

        }
    }
}
