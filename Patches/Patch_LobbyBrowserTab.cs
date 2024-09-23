using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using System.Collections;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    internal class Patch_LobbyBrowserTab
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyBrowserTab), nameof(LobbyBrowserTab.UpdateTab))]
        private static bool UpdateTab_Prefix(LobbyBrowserTab __instance)
        {
            // Apply Grid Layout Height
            __instance.SetGridlayoutHeight();

            // Run new Coroutine
            __instance.StartCoroutine(UpdateLobbyList(__instance));

            // Prevent Original
            return false;
        }

        private static IEnumerator UpdateLobbyList(LobbyBrowserTab __instance)
        {
            // Request Lobby List
            TransportSwitcher.instance.SwitchTransportToFizzySteamworks();
            SteamLobby.instance.GetLobbyList(true);
            while (!SteamLobby.instance.receivedLobbyList)
                yield return null;

            // Destroy All Old Listings
            foreach (Transform obj in __instance.lobbyListContent)
                Object.Destroy(obj.gameObject);

            // Process through Lobbies Found
            uint lobbyCount = SteamLobby.instance.lobbyList.m_nLobbiesMatching;
            string clientVersion = Application.version;
            for (int i = 0; i < lobbyCount; i++)
            {
                // Get Lobby Id
                CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);

                // Get Lobby Information
                string lobbyName = SteamMatchmaking.GetLobbyData(lobbyId, "NAME");
                string lobbyState = SteamMatchmaking.GetLobbyData(lobbyId, "STATE");
                string lobbyType = SteamMatchmaking.GetLobbyData(lobbyId, "TYPE");
                string lobbyPrivacy = SteamMatchmaking.GetLobbyData(lobbyId, "PRIVACY");
                string lobbyVersion = SteamMatchmaking.GetLobbyData(lobbyId, "VERSION");

                // Create Lobby Listing
                if (lobbyState.Equals("WAITING")
                    && lobbyType.Equals("MODDED")
                    && lobbyPrivacy.Equals("PUBLIC")
                    && lobbyVersion.Equals(clientVersion))
                    Object.Instantiate(__instance.lobbyItemPrefab,
                        __instance.lobbyListContent).GetComponent<LobbyItem>()
                        .SetLobby(lobbyId, lobbyName);
            }

            // Apply Grid Layout Height
            __instance.SetGridlayoutHeight();
            yield break;
        }
    }
}