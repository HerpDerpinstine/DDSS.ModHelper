using DDSS_ModHelper.Components;
using DDSS_ModHelper.Console;
using DDSS_ModHelper.Patches;
using DDSS_ModHelper.Settings;
using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2CppMirror;
using MelonLoader;
using System;
using System.Reflection;

namespace DDSS_ModHelper
{
    internal class MelonMain : MelonMod
    {
        internal static MelonLogger.Instance _logger;
        internal static bool _errorOccured;

        public override void OnInitializeMelon()
        {
            // Static Cache Logger
            _logger = LoggerInstance;

            // Create Preferences
            ConfigHandler.Setup();

            // Register Custom Types
            CustomConsoleHandler.Register();
            ManagedEnumerator.Register();

            // Apply Patches
            ApplyPatches();

            // Create Default Commands
            ConsoleManager.AddCommand<HelpCommand>();
            ConsoleManager.AddCommand<ClearCommand>();
			
            // Prevent Exceptions from causing Disconnects
            NetworkServer.exceptionsDisconnect = false;
            NetworkClient.exceptionsDisconnect = false;

            // Log Success
            _logger.Msg("Initialized!");
        }

        private void ApplyPatches()
        {
            Assembly melonAssembly = typeof(MelonMain).Assembly;
            foreach (Type type in melonAssembly.GetValidTypes())
            {
                // Check Type for any Harmony Attribute
                if (type.GetCustomAttribute<HarmonyPatch>() == null)
                    continue;

                // Apply
                try
                {
                    if (MelonDebug.IsEnabled())
                        LoggerInstance.Msg($"Applying {type.Name}");

                    HarmonyInstance.PatchAll(type);
                }
                catch (Exception e)
                {
                    _errorOccured = true;
                    LoggerInstance.Error($"Exception while attempting to apply {type.Name}: {e}");
                }
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            ConsoleManager.OnSceneInit();

            if (sceneName == "MainMenuScene") // Main Menu
            {
                ModSettingsManager.MainMenuInit();
            }
            else if (sceneName == "LobbyScene") // Lobby awaiting Start
            {

            }
            else // In-Game
            {
                ModSettingsManager.GameInit();
            }
        }

        private void ApplyPatch<T>()
        {
            Type type = typeof(T);
            try
            {
                HarmonyInstance.PatchAll(type);
            }
            catch (Exception e)
            {
                LoggerInstance.Error($"Exception while attempting to apply {type.Name}: {e}");
            }
        }
    }   
}
