using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.SerializeSyncVars))]
        private static void SerializeSyncVars_Postfix(NetworkWriter __0, bool __1)
        {
            // Check for Host
            if (__1
                || (__0 == null)
                || __0.WasCollected)
                return;

            // Write Extra Byte to Signify Modded Client
            __0.WriteByte(1);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Postfix(LobbyPlayer __instance, NetworkReader __0, bool __1)
        {
            // Check for Host
            if (__1
                || (__0 == null)
                || __0.WasCollected)
                return;

            // Read Extra Byte to Signify Modded Client
            try
            {
                byte moddedByte = __0.ReadByte();
                if (moddedByte != 1)
                    __instance.connectionToClient.Disconnect();
            }
            catch
            {
                __instance.connectionToClient.Disconnect();
            }
        }
    }
}
