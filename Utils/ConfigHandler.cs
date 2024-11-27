using MelonLoader;
using UnityEngine;

namespace DDSS_ModHelper.Utils
{
    internal static class ConfigHandler
    {
        private static MelonPreferences_Category _prefs_Category;
        internal static MelonPreferences_Entry<bool> _prefs_DevConsole;
        internal static MelonPreferences_Entry<KeyCode> _prefs_DevConsoleKeybind;

        internal static void Setup()
        {
            // Create Preferences Category
            _prefs_Category = MelonPreferences.CreateCategory("ModHelper", "Mod Helper");

            // Create Preferences Entries
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
