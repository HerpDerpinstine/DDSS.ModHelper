﻿using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_LocalizationManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.GetLocalizedValue), typeof(string))]
        private static bool GetLocalizedValue_1_Prefix(string __0)
        {
            // Validate Key
            if (string.IsNullOrEmpty(__0))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.GetLocalizedValue), typeof(string), typeof(Il2CppStringArray))]
        private static bool GetLocalizedValue_2_Prefix(string __0)
        {
            // Validate Key
            if (string.IsNullOrEmpty(__0))
                return false;

            // Run Original
            return true;
        }
    }
}