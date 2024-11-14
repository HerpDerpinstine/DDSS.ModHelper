using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    [HarmonyPatch]
    internal class Patch_StartGameState
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartGameState), nameof(StartGameState.Enter))]
        private static bool Enter_Prefix(StartGameState __instance)
        {
            // Check if Server
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
            int playerCount = LobbyManager.instance.connectedLobbyPlayers.Count;
            Il2CppSystem.Collections.Generic.List<NetworkIdentity> playerList = new();
            NetworkIdentity playerManager = null;
            foreach (NetworkIdentity player in LobbyManager.instance.connectedLobbyPlayers)
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
    }
}
