using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using Il2CppUMUI;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_SteamLobby
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.OnLobbyCreated))]
        private static void OnLobbyCreated_Prefix(LobbyCreated_t __0)
        {
            // Create CSteamID
            CSteamID lobbySteamId = new(__0.m_ulSteamIDLobby);

            // Apply Custom Lobby Data
            SteamMatchmaking.SetLobbyData(lobbySteamId, "VERSION", Application.version);
            SteamMatchmaking.SetLobbyData(lobbySteamId, "REQUIREMENTS", RequirementFilterHandler.Generate());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.JoinLobby))]
        private static bool JoinLobby_Prefix(SteamLobby __instance, CSteamID __0)
        {
            // Validate Objects
            if ((ErrorManager.instance == null)
               || ErrorManager.instance.WasCollected
               || (UIManager.instance == null)
               || UIManager.instance.WasCollected
               || (LocalizationManager.instance == null)
               || LocalizationManager.instance.WasCollected
               || (LobbyManager.instance == null)
               || LobbyManager.instance.WasCollected
               || LobbyManager.instance.isServer)
               return true;

            // Get Server Version
            string serverVersion = SteamMatchmaking.GetLobbyData(__0, "VERSION");
            if (string.IsNullOrEmpty(serverVersion))
            {
                // Disconnect
                ErrorDisconnect(__instance,
                    "Verification Failure",
                    "Failed to get Game Version from Server!");

                // Prevent Original
                return false;
            }

            // Check Client and Server Versions
            string clientVersion = Application.version;
            if (serverVersion != clientVersion)
            {
                // Disconnect
                ErrorDisconnect(__instance,
                    "Game Version Mismatch",
                    $"You are on {clientVersion} but the Lobby Host is on {serverVersion}!");

                // Prevent Original
                return false;
            }

            // Get Requirements List
            string requirementsJson = SteamMatchmaking.GetLobbyData(__0, "REQUIREMENTS");
            if (string.IsNullOrEmpty(requirementsJson))
            {
                // Disconnect
                ErrorDisconnect(__instance,
                    "Verification Failure",
                    "Failed to get Mods from Server!");

                // Prevent Original
                return false;
            }

            // Check Mod List
            RequirementFilterHandler.Parse(requirementsJson,
                out List<RequirementFilterHandler.SerializedRequirement> missingMods,
                out List<RequirementFilterHandler.SerializedRequirement> mismatchedMods);
            if ((missingMods.Count > 0) || (mismatchedMods.Count > 0))
            {
                // Disconnect
                ErrorDisconnect(__instance,
                    "Dependencies Mismatch",
                    "You are not running the required Plugins, Mods, or Custom Content for this Lobby!");

                // Prevent Original
                return false;
            }

            // Run Original
            return true;
        }

        private static void ErrorDisconnect(SteamLobby lobby, string header, string content)
        {
            // Leave Lobby
            lobby.LeaveLobby();

            // Display Popup
            UIManager.instance.CloseLoadingScreen();
            ErrorManager.instance.ShowPopupOnLoadMainMenu(header, content,
                LocalizationManager.instance.GetLocalizedValue("Ok!"), "error");
        }
    }
}