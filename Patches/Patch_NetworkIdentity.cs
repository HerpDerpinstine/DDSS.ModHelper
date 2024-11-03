using HarmonyLib;
using Il2CppMirror;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_NetworkIdentity
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NetworkIdentity), nameof(NetworkIdentity.SerializeClient))]
        private static void SerializeClient_Postfix(NetworkWriter __0)
        {
            // Write Extra Byte to Signify Modded Client
            __0.WriteByte(1);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NetworkIdentity), nameof(NetworkIdentity.DeserializeServer))]
        private static void DeserializeServer_Postfix(NetworkIdentity __instance,
            NetworkReader __0,
            ref bool __result)
        {
            // Check if Original Failed
            if (!__result)
                return;

            // Read Extra Byte to Signify Modded Client
            try
            {
                byte moddedByte = __0.ReadByte();
                if (moddedByte != 1)
                {
                    __instance.connectionToClient.Disconnect();
                    __result = false;
                }
            }
            catch
            {
                __instance.connectionToClient.Disconnect();
                __result = false;
            }
        }
    }
}