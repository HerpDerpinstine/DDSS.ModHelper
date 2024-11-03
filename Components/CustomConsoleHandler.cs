using DDSS_ModHelper.Console;
using DDSS_ModHelper.Settings;
using DDSS_ModHelper.Utils;
using Il2CppInterop.Runtime.Injection;
using Il2CppUI.Console;
using Il2CppUMUI;
using System;
using UnityEngine;

namespace DDSS_ModHelper.Components
{
    public class CustomConsoleHandler : MonoBehaviour
    {
        private int historyIndex;
        private static bool _wasActive;

        internal static void Register()
            => ClassInjector.RegisterTypeInIl2Cpp<CustomConsoleHandler>();
        public CustomConsoleHandler(IntPtr ptr) : base(ptr) { }

        public void Awake()
        {
            ConsoleController controller = gameObject.GetComponent<ConsoleController>();
            if ((ConsoleController.instance != null)
                && !ConsoleController.instance.WasCollected
                && (ConsoleController.instance != controller))
            {
                GameObject.Destroy(gameObject);
                return;
            }

            ConsoleController.instance = controller;
            ConsoleManager.ApplyConsoleTextToObject();
            historyIndex = ConsoleManager._cmdHistory.Count;
            ConsoleController.instance.consolePanel.SetActive(_wasActive);
            if (_wasActive)
            {
                UIManager.instance.isLocked = true;
                ConsoleController.instance.inputField.ActivateInputField();
            }
        }

        public void Update()
        {
            // Validate Objects
            if ((UIManager.instance == null)
                || UIManager.instance.WasCollected
                || (ConsoleController.instance == null)
                || ConsoleController.instance.WasCollected
                || (ConsoleController.instance.consolePanel == null)
                || ConsoleController.instance.consolePanel.WasCollected
                || (ConsoleController.instance.inputField == null)
                || ConsoleController.instance.inputField.WasCollected)
                return;

            // Check Toggle and Keybind
            if (ConfigHandler._prefs_DevConsole.Value)
            {
                if ((ModSettingsManager._rebindCoroutine == null)
                    && Input.GetKeyDown(ConfigHandler._prefs_DevConsoleKeybind.Value))
                {
                    ConsoleController.instance.consolePanel.SetActive(!ConsoleController.instance.consolePanel.activeSelf);
                    if (ConsoleController.instance.consolePanel.activeSelf)
                    {
                        UIManager.instance.isLocked = true;
                        ConsoleController.instance.inputField.ActivateInputField();
                    }
                    else
                    {
                        UIManager.instance.isLocked = false;
                        ConsoleController.instance.inputField.DeactivateInputField();
                    }
                }

                if ((ConsoleController.instance.cheatModeLabel != null)
                    && !ConsoleController.instance.cheatModeLabel.WasCollected
                    && !ConsoleController.instance.cheatModeLabel.activeSelf)
                    ConsoleController.instance.cheatModeLabel.SetActive(true);
            }
            else
            {
                if (ConsoleController.instance.consolePanel.activeSelf)
                {
                    ConsoleController.instance.consolePanel.SetActive(false);
                    UIManager.instance.isLocked = false;
                    ConsoleController.instance.inputField.DeactivateInputField();
                }

                if ((ConsoleController.instance.cheatModeLabel != null)
                    && !ConsoleController.instance.cheatModeLabel.WasCollected
                    && ConsoleController.instance.cheatModeLabel.activeSelf)
                    ConsoleController.instance.cheatModeLabel.SetActive(false);
            }

            // Check if Console is Shown
            if (!ConsoleController.instance.consolePanel.activeSelf)
            {
                _wasActive = false;
                return;
            }
            _wasActive = true;

            // Apply and Clamp History Index
            void ApplyHistory()
            {
                historyIndex = Mathf.Clamp(historyIndex, 0, ConsoleManager._cmdHistory.Count - 1);
                if (ConsoleManager._cmdHistory.Count > 0)
                    ConsoleController.instance.inputField.text = ConsoleManager._cmdHistory[historyIndex];
            }

            // Check Up History Keybind
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                historyIndex--;
                ApplyHistory();
            }

            // Check Down History Keybind
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                historyIndex++;
                ApplyHistory();
            }

            // Check Command Execution Keybind
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Execute Command
                try
                {
                    // Validate Length
                    string fullCmd = ConsoleController.instance.inputField.text;
                    if (!string.IsNullOrEmpty(fullCmd)
                        && !string.IsNullOrWhiteSpace(fullCmd))
                    {
                        // Run Command
                        ConsoleManager.ExecuteCommand(fullCmd);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleManager.PrintError(ex.ToString());
                }

                // Clear Input Field
                historyIndex = ConsoleManager._cmdHistory.Count;
                ConsoleController.instance.inputField.text = string.Empty;
                ConsoleController.instance.inputField.ActivateInputField();
            }
        }
    }
}
