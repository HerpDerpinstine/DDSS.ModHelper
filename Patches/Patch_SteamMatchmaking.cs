using HarmonyLib;
using Il2CppSteamworks;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    internal class Patch_SteamMatchmaking
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamMatchmaking), nameof(SteamMatchmaking.RequestLobbyList))]
        private static void RequestLobbyList_Prefix()
        {
            // Set VERSION filter
            SteamMatchmaking.AddRequestLobbyListStringFilter("VERSION", Application.version, ELobbyComparison.k_ELobbyComparisonEqual);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamMatchmaking), nameof(SteamMatchmaking.AddRequestLobbyListStringFilter))]
        private static void AddRequestLobbyListStringFilter_Prefix(string __0, ref string __1, ref ELobbyComparison __2)
        {
            // Check for TYPE filter
            if (__0 != "TYPE")
                return;

            // Set PRIVACY filter with TYPE filter value
            SteamMatchmaking.AddRequestLobbyListStringFilter("PRIVACY", __1, ELobbyComparison.k_ELobbyComparisonEqual);

            // Change TYPE filter value for MODDED pool
            __1 = "MODDED";
            __2 = ELobbyComparison.k_ELobbyComparisonEqual;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamMatchmaking), nameof(SteamMatchmaking.SetLobbyData))]
        private static void SetLobbyData_Prefix(CSteamID __0, string __1, ref string __2)
        {
            // Check for TYPE filter
            if (__1 != "TYPE")
                return;

            // Set PRIVACY filter with TYPE filter value
            SteamMatchmaking.SetLobbyData(__0, "PRIVACY", __2);

            // Change TYPE filter value for MODDED pool
            __2 = "MODDED";
        }
    }
}