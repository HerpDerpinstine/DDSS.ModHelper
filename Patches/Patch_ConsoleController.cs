using DDSS_ModHelper.Settings;
using DDSS_ModHelper.Utils;
using HarmonyLib;
using Il2CppUI.Console;
using Il2CppUMUI;
using System;
using UnityEngine;

namespace DDSS_ModHelper.Patches
{
    internal class Patch_ConsoleController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.PrintErrorToConsole))]
        private static bool PrintErrorToConsole_Prefix(string __0)
        {
            // Log Message
            MelonMain._logger.Error(__0);

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.PrintToConsole), [typeof(string)])]
        private static bool PrintToConsole_Prefix_1(string __0)
        {
            // Log Message
            MelonMain._logger.Msg(__0);

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.PrintToConsole), [typeof(string), typeof(Color)])]
        private static bool PrintToConsole_Prefix_2(string __0, Color __1)
        {
            // Log Message
            MelonMain._logger.Msg(__1.ToDrawingColor(), __0);

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConsoleController), nameof(ConsoleController.Update))]
        private static bool Update_Prefix(ConsoleController __instance)
        {
            // Validate Objects
            if ((UIManager.instance == null)
                || UIManager.instance.WasCollected
                || (__instance.consolePanel == null)
                || __instance.consolePanel.WasCollected
                || (__instance.inputField == null)
                || __instance.inputField.WasCollected
                || (__instance.history == null)
                || __instance.history.WasCollected)
                return false;

            // Check Toggle and Keybind
            if (ConfigHandler._prefs_DevConsole.Value)
            {
                if ((ModSettingsManager._rebindCoroutine == null)
                    && Input.GetKeyDown(ConfigHandler._prefs_DevConsoleKeybind.Value))
                {
                    __instance.consolePanel.SetActive(!__instance.consolePanel.activeSelf);
                    if (__instance.consolePanel.activeSelf)
                    {
                        UIManager.instance.isLocked = true;
                        __instance.inputField.ActivateInputField();
                    }
                    else
                    {
                        UIManager.instance.isLocked = false;
                        __instance.inputField.DeactivateInputField();
                    }
                }
            }
            else
            {
                if (__instance.consolePanel.activeSelf)
                {
                    __instance.consolePanel.SetActive(false);
                    UIManager.instance.isLocked = false;
                    __instance.inputField.DeactivateInputField();
                }
            }

            // Check if Console is Shown
            if (!__instance.consolePanel.activeSelf)
                return false;

            // Apply and Clamp History Index
            void ApplyHistory()
            {
                __instance.historyIndex = Mathf.Clamp(__instance.historyIndex, 0, __instance.history.Count - 1);
                if (__instance.history.Count > 0)
                    __instance.inputField.text = __instance.history[__instance.historyIndex];
            }

            // Check Up History Keybind
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                __instance.historyIndex--;
                ApplyHistory();
            }

            // Check Down History Keybind
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                __instance.historyIndex++;
                ApplyHistory();
            }

            // Check Command Execution Keybind
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Execute Command
                try
                {
                    __instance.ExecuteCommand(__instance.inputField.text);
                }
                catch (Exception ex)
                {
                    __instance.PrintErrorToConsole($"Error: {ex}");
                }

                // Clear Input Field
                __instance.historyIndex = __instance.history.Count;
                __instance.inputField.text = string.Empty;
                __instance.inputField.ActivateInputField();
            }

            // Prevent Original
            return false;
        }
    }
}