using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppLocalization;
using Il2CppSteamworks;
using Il2CppTMPro;
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

            // Get Header Text
            Transform titleTrans = __instance.transform.Find("Tab/LobbyList/TopBar/Title");
            if (titleTrans != null)
            {
                // Apply Header Text
                TextMeshProUGUI titleText = titleTrans.GetComponentInChildren<TextMeshProUGUI>();
                if (titleText != null)
                    titleText.text = "Modded Lobby Browser";

                // Remove Localization
                LocalizedText localized = titleTrans.GetComponentInChildren<LocalizedText>();
                if (localized != null)
                    UnityEngine.Object.Destroy(localized);
            }

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
            int childCount = __instance.lobbyListContent.childCount;
            if (childCount > 0)
                for (int i = 0; i < childCount; i++)
                    GameObject.Destroy(__instance.lobbyListContent.GetChild(i).gameObject);

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