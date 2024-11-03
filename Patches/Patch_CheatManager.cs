using HarmonyLib;
using Il2CppGameManagement;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_CheatManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CheatManager), nameof(CheatManager.Awake))]
        private static bool Awake_Prefix(CheatManager __instance)
        {
            // Set instance
            if (CheatManager.instance != null)
            {
                Object.Destroy(__instance.gameObject);
                return false;
            }
            CheatManager.instance = __instance;

            // Set Custom Values
            __instance.isCheatMode = true;
            __instance.noLoading = true;

            // Prevent Object Destruction
            Object.DontDestroyOnLoad(__instance);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CheatManager), nameof(CheatManager.Update))]
        private static bool Update_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}