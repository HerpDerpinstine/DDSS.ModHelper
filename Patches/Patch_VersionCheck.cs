using HarmonyLib;
using Il2Cpp;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_VersionCheck
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VersionCheck), nameof(VersionCheck.Start))]
        private static bool Start_Prefix()
        {
            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VersionCheck), nameof(VersionCheck.CheckGameVersion))]
        private static bool CheckGameVersion_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}