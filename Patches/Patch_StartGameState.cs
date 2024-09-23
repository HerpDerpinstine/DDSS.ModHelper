using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using Il2CppUMUI;
using System.Collections;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    internal class Patch_StartGameState
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartGameState), nameof(StartGameState.Enter))]
        private static bool Enter_Prefix(StartGameState __instance)
        {// Check if Server
            if (!__instance.gameManager.isServer)
                return false;

            // Get List of Spawn Points
            Il2CppSystem.Collections.Generic.List<NetworkStartPosition> spawnList = new();
            foreach (NetworkStartPosition spawn in
                UnityEngine.Object.FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.None))
                spawnList.Add(spawn);
            int spawnCount = spawnList.Count;

            // Get List of Workstations
            int workstationCount = __instance.gameManager.workStations.Count;
            Il2CppSystem.Collections.Generic.List<WorkStationController> workstationList = new();
            foreach (WorkStationController workStationController in __instance.gameManager.workStations)
            {
                // Skip Manager Workstation
                if (workStationController == __instance.gameManager.managerWorkStationController)
                    continue;

                // Add Workstation to List
                workstationList.Add(workStationController);
            }

            // Get List of Players
            int playerCount = LobbyManager.instance.connectedPlayers.Count;
            Il2CppSystem.Collections.Generic.List<NetworkIdentity> playerList = new();
            NetworkIdentity playerManager = null;
            foreach (NetworkIdentity player in LobbyManager.instance.connectedPlayers)
            {
                // Validate Lobby Player
                LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
                if (lobbyPlayer == null)
                    continue;

                // Check for Manager
                if (lobbyPlayer.playerRole == PlayerRole.Manager)
                {
                    // Cache Manager Player and Skip
                    playerManager = player;
                    continue;
                }

                // Add to List
                playerList.Add(player);
            }

            // Randomize Lists
            __instance.gameManager.ShuffleList(workstationList);
            __instance.gameManager.ShuffleList(playerList);
            __instance.gameManager.ShuffleList(spawnList);

            // Re-Add Manager Player to First in Player List
            if (playerManager != null)
                playerList.Insert(0, playerManager);

            // Iterate through Players
            int slackerCount = 0;
            int specialistCount = 0;
            int slackerAmount = __instance.gameManager.slackerAmount;
            for (int i = 0; i < playerCount; i++)
            {
                // Get Player
                NetworkIdentity player = playerList[i];
                LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();

                // Clear Tasks for Player
                player.GetComponent<TaskController>().RpcClearTaskQueue();

                // Move Player to Spawn
                lobbyPlayer.NetworkplayerController.GetComponent<PlayerController>()
                    .CmdMovePlayer(spawnList[i % spawnCount].transform.position);

                // Assign Player Role and Workstation
                if (i == 0) // Manager
                {
                    lobbyPlayer.ServerSetPlayerRole(PlayerRole.Manager);
                    lobbyPlayer.ServerSetWorkStation(__instance.gameManager.managerWorkStationController, PlayerRole.Manager, true);
                }
                else if (slackerCount <= slackerAmount) // Slacker
                {
                    slackerCount++;
                    lobbyPlayer.ServerSetPlayerRole(PlayerRole.Slacker);
                    lobbyPlayer.ServerSetWorkStation(workstationList[i % workstationCount], PlayerRole.Slacker, true);
                }
                else // Specialist
                {
                    specialistCount++;
                    lobbyPlayer.ServerSetPlayerRole(PlayerRole.Specialist);
                    lobbyPlayer.ServerSetWorkStation(workstationList[i % workstationCount], PlayerRole.Specialist, true);
                }
            }

            // Apply Win Condition
            __instance.gameManager.RpcResetTerminationTimer(__instance.gameManager.terminationMaxTime);
            __instance.gameManager.SetWinCondition(specialistCount, slackerCount);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartGameState), nameof(StartGameState.EnterCoroutine))]
        private static bool EnterCoroutine_Prefix(StartGameState __instance)
        {
            // Run New Coroutine
            __instance.gameManager.StartCoroutine(FixedEnterCoroutine(__instance));

            // Prevent Original
            return false;
        }

        private static IEnumerator FixedEnterCoroutine(StartGameState __instance)
        {
            // Validate Local Player Role
            while (LobbyManager.instance.GetLocalPlayer().playerRole == PlayerRole.None)
                yield return null;

            // Show Intro
            if (!ConfigHandler._prefs_SkipIntro.Value)
            {
                UIManager.instance.OpenTab("Intro");
                yield return new WaitForSeconds(7.5f);
            }

            // Close All Tabs
            UIManager.instance.CloseAllTabs();
            yield return new WaitForSeconds(0.1f);

            // Reveal Role
            UIManager.instance.OpenTab("ShowRole");
            yield return new WaitForSeconds(4f);

            // Close All Tabs
            UIManager.instance.CloseAllTabs();
            yield return new WaitForSeconds(0.1f);

            // Show HUD
            UIManager.instance.OpenTab("HUD");

            // Role Hint
            switch (LobbyManager.instance.GetLocalPlayer().playerRole)
            {
                case PlayerRole.Specialist:
                    UIManager.instance.ShowHint(
                        LocalizationManager.instance.GetLocalizedValue(
                            "You are a specialist! Your job is to complete the tasks given to you by the manager. Also, it might be a good idea to look out for the slackers in order to tell the manager!"), 
                        8f, 12f);
                    break;

                case PlayerRole.Slacker:
                    UIManager.instance.ShowHint(
                        LocalizationManager.instance.GetLocalizedValue(
                        "You are a slacker! Your job is to pointless tasks without making the manager suspicious. You can do that by avoiding the manager and not getting fired."),
                        8f, 12f);
                    break;

                case PlayerRole.Manager:
                    UIManager.instance.ShowHint(
                        LocalizationManager.instance.GetLocalizedValue(
                            "You are the manager! Your job is to figure out who the slackers are and fire them. You can do that using the whiteboard in the conference room!"), 
                        8f, 12f);
                    break;
            }

            // Firewall Hint
            UIManager.instance.ShowHint(
                LocalizationManager.instance.GetLocalizedValue(
                    "Remember to turn on the firewall on your computer! Otherwise, you might get a virus. However, slackers get viruses regardless"),
                8f, 30f);
            
            // Signal that the Match has Started
            if (__instance.gameManager.isServer)
                __instance.ChangeState(GameStates.InGame);

            // Construct Map
            MapCreator.instance.ConstructMap();

            // Break Coroutine
            yield break;
        }
    }
}
