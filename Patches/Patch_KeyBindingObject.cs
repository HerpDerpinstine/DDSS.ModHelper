using HarmonyLib;
using Il2CppUI.Tabs.SettingsTab;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_KeyBindingObject
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyBindingObject), nameof(KeyBindingObject.SetKeyBind))]
        private static void SetKeyBind_Prefix(KeyBindingObject __instance)
        {
            MelonMain._isBindingKey = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyBindingObject), nameof(KeyBindingObject.RefreshKeyBindText))]
        private static void RefreshKeyBindText_Prefix(KeyBindingObject __instance)
        {
            MelonMain._isBindingKey = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyBindingObject), nameof(KeyBindingObject.UpdateKeyBind))]
        private static void UpdateKeyBind_Prefix(KeyBindingObject __instance)
        {
            MelonMain._isBindingKey = true;
        }
    }
}