using DDSS_ModHelper.Console;
using HarmonyLib;
using Il2CppUI.Console;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    internal class Patch_ConsoleController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.PrintErrorToConsole))]
        private static bool PrintErrorToConsole_Prefix(ConsoleController __instance, string __0)
        {
            // Print to Console
            ConsoleManager.PrintError(__0);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.PrintToConsole), [typeof(string)])]
        private static bool PrintToConsole_Prefix_1(string __0)
        {
            // Print to Console
            ConsoleManager.PrintMsg(__0);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.PrintToConsole), [typeof(string), typeof(Color)])]
        private static bool PrintToConsole_Prefix_2(string __0, Color __1)
        {
            // Print to Console
            ConsoleManager.PrintMsg(__1, __0);

            // Prevent Original
            return false;
        }
    }
}