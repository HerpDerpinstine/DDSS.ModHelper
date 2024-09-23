using MelonLoader;
using UnityEngine;

namespace DDSS_ModHelper.Utils
{
    internal static class ConfigHandler
    {
        private static MelonPreferences_Category _prefs_Category;
        internal static MelonPreferences_Entry<bool> _prefs_SkipIntro;
        internal static MelonPreferences_Entry<AlternateStyleHandler.eLogoStyle> _prefs_MenuLogoStyle;
        internal static MelonPreferences_Entry<AlternateStyleHandler.eBackgroundStyle> _prefs_MenuBackgroundStyle;
        internal static MelonPreferences_Entry<AlternateStyleHandler.eBackgroundStyle> _prefs_LobbyBackgroundStyle;
        internal static MelonPreferences_Entry<bool> _prefs_DevConsole;
        internal static MelonPreferences_Entry<KeyCode> _prefs_DevConsoleKeybind;

        internal static void Setup()
        {
            // Create Preferences Category
            _prefs_Category = MelonPreferences.CreateCategory("ModHelper", "Mod Helper");

            // Create Preferences Entries
            _prefs_SkipIntro = CreatePref("SkipIntro",
                "Skip Intro",
                "Skips the Intro when a Match starts",
                false);

            _prefs_MenuLogoStyle = CreatePref("MenuLogoStyle",
                "Menu Logo Style",
                "Sets the Style of the Game Logo on the Main Menu",
                AlternateStyleHandler.eLogoStyle.Transparent);

            _prefs_MenuBackgroundStyle = CreatePref("MenuBackgroundStyle",
                "Menu Background Style",
                "Sets the Style of the Game Background on the Main Menu",
                AlternateStyleHandler.eBackgroundStyle.Gradient);

            _prefs_LobbyBackgroundStyle = CreatePref("LobbyBackgroundStyle",
                "Lobby Background Style",
                "Sets the Style of the Game Background on the Lobby",
                AlternateStyleHandler.eBackgroundStyle.Flat);

            _prefs_DevConsole = CreatePref("DevConsole",
                "Developer Console",
                "Toggle for the Developer Console",
                false);

            _prefs_DevConsoleKeybind = CreatePref("DevConsoleKeybind",
                "Developer Console Keybind",
                "Keybind for toggling the Developer Console",
                KeyCode.F1);
        }

        private static MelonPreferences_Entry<T> CreatePref<T>(
            string id,
            string displayName,
            string description,
            T defaultValue,
            bool isHidden = false)
            => _prefs_Category.CreateEntry(id,
                defaultValue,
                displayName,
                description,
                isHidden,
                false,
                null);
    }
}
