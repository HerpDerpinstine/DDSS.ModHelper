using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2CppUMUI;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_UIManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.Awake))]
        private static void Awake_Postfix()
        {
            AlternateStyleHandler.Apply();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.OpenTab))]
        private static bool OpenTab_Prefix(string __0)
        {
            if ((__0 == "Menu")
                || (__0 == "Lobby"))
                AlternateStyleHandler.Apply();

            // Run Original
            return true;
        }
    }
}
