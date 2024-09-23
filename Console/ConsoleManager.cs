using Il2CppUI.Console;
using UnityEngine;

namespace DDSS_ModHelper.Console
{
    internal static class ConsoleManager
    {
        internal static void MainMenuInit()
        {
            // Allow Developer Console in Main Menu
            ConsoleController[] comps = Resources.FindObjectsOfTypeAll<ConsoleController>();
            foreach (ConsoleController comp in comps)
            {
                comp.gameObject.SetActive(true);
                break;
            }
        }
    }
}
