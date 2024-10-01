using DDSS_ModHelper.Components;
using DDSS_ModHelper.Console;
using DDSS_ModHelper.Patches;
using DDSS_ModHelper.Settings;
using DDSS_ModHelper.Utils;
using MelonLoader;
using System;

namespace DDSS_ModHelper
{
    internal class MelonMain : MelonMod
    {
        internal static MelonLogger.Instance _logger;

        public override void OnInitializeMelon()
        {
            // Static Cache Logger
            _logger = LoggerInstance;

            // Create Preferences
            ConfigHandler.Setup();

            // Register Custom Types
            ManagedEnumerator.Register();

            // Apply Patches
            ApplyPatch<Patch_CheatManager>();
            ApplyPatch<Patch_ConsoleController>();
            ApplyPatch<Patch_KeyBindingObject>();
            ApplyPatch<Patch_LobbyBrowserTab>();
            ApplyPatch<Patch_LocalizationManager>();
            ApplyPatch<Patch_NetworkIdentity>();
            ApplyPatch<Patch_SettingObject>();
            ApplyPatch<Patch_SettingsManager>();
            ApplyPatch<Patch_SettingsTab>();
            ApplyPatch<Patch_StartGameState>();
            ApplyPatch<Patch_SteamLobby>();
            ApplyPatch<Patch_SteamMatchmaking>();
            ApplyPatch<Patch_UIManager>();
            ApplyPatch<Patch_VersionCheck>();

            // Log Success
            _logger.Msg("Initialized!");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenuScene") // Main Menu
            {
                ConsoleManager.MainMenuInit();
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
