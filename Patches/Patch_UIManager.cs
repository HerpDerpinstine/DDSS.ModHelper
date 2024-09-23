using DDSS_ModHelper.Settings;
using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2CppUMUI;

namespace DDSS_ModHelper.Patches
{
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

            // Check if Rebinding
            if (ModSettingsManager._rebindCoroutine != null)
            {
                ModSettingsManager.CancelRebind();
                return false;
            }

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.OpenFirstTab))]
        private static bool OpenFirstTab_Prefix()
        {
            // Check if Rebinding
            if (ModSettingsManager._rebindCoroutine != null)
            {
                ModSettingsManager.CancelRebind();
                return false;
            }

            // Reun Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.CloseTopTab))]
        private static bool CloseTopTab_Prefix()
        {
            // Check if Rebinding
            if (ModSettingsManager._rebindCoroutine != null)
            {
                ModSettingsManager.CancelRebind();
                return false;
            }

            // Reun Original
            return true;
        }
    }
}
