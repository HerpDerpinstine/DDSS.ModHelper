using HarmonyLib;
using Il2CppLocalization;
using Il2CppTMPro;

namespace DDSS_ModHelper.Patches
{
    internal class Patch_LocalizedText
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalizedText), nameof(LocalizedText.Start))]
        private static void Start_Postfix(LocalizedText __instance)
        {
            // Get TextMeshPro Text Component
            TMP_Text component = __instance.GetComponent<TMP_Text>();
            if (component == null)
                return;

            // Apply Prefix to Lobby Browser Header
            if (__instance.originalText == "Lobby Browser")
                component.text = "Modded Lobby Browser";
        }
    }
}