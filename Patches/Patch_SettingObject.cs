using HarmonyLib;
using Il2Cpp;
using Il2CppUI.Tabs.SettingsTab;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_SettingObject
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingObject), nameof(SettingObject.SetSetting))]
        private static void SetSetting_Prefix(Setting __0)
        {
            // Check for Max Players
            if (__0.Key == "Max players")
            {
                // Change Max Value to 99
                __0.alternatives[1].key = "99";
                __0.alternatives[1].label = "99";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingObject), nameof(SettingObject.ApplySetting))]
        private static bool ApplySetting_Prefix(SettingObject __instance, ref bool __0)
        {
            // Apply Value
            if (__instance.setting.Value != __instance.value)
                __instance.setting.Value = __instance.value;

            // Preset Verification
            if (__0
                && (__instance.setting.presetName != string.Empty)
                && !__instance.blockPresets)
                __instance.settingsManager.currentPreset.VerifyPreset(
                    __instance.setting,
                    __instance.settingsManager);

            // Prevent Original
            return false;
        }
    }
}