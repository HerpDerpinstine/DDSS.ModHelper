using DDSS_ModHelper.Settings;
using DDSS_ModHelper.Settings.Internal;
using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppUI.Tabs.SettingsTab;
using MelonLoader;
using System.Collections.Generic;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_SettingsTab
    {
        private static List<MelonPreferences_Category> _categoryCache = new();
        private static Dictionary<SettingObject, MelonPreferences_Entry> _settingCache = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ShowSettings))]
        private static bool ShowSettings_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Generate Entries
            ModSettingsFactory.Generate();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ShowCategories))]
        private static bool ShowCategories_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ApplyAllSettings))]
        private static bool ApplyAllSettings_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Apply Changes
            ModSettingsFactory.Apply();
            AlternateStyleHandler.Apply();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ResetAllSettings))]
        private static bool ResetAllSettings_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Reset to Default
            ModSettingsFactory.Reset();
            AlternateStyleHandler.Apply();

            // Prevent Original
            return false;
        }
    }
}