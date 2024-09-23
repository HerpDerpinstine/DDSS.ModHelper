using Il2CppLocalization;
using Il2CppTMPro;
using Il2CppUMUI.UiElements;
using System;
using UnityEngine;

namespace DDSS_ModHelper.Settings.Internal
{
    internal static class ModSettingsButtonBuilder
    {
        internal static void MainMenuInit()
            => Create("Canvas/MenuTab/Tab/Tasks/TopBar/Box/Settings", true);

        internal static void GameInit()
        {
            // Fix Pause Menu Delta
            GameObject pauseMenuObj = GameObject.Find("Persistent/UIManager/PauseMenuTab/Tab/Tasks");
            if (pauseMenuObj != null)
            {
                RectTransform trans = pauseMenuObj.GetComponent<RectTransform>();
                Vector2 sizeDelta = trans.sizeDelta;
                sizeDelta.y += 60;
                trans.sizeDelta = sizeDelta;
            }

            // Create Button
            Create("Persistent/UIManager/PauseMenuTab/Tab/Tasks/Box/Grid/Settings", false);
        }

        private static void Create(string buttonPath, bool parentOfParent)
        {
            // Find Settings Button
            GameObject settingsObj = GameObject.Find(buttonPath);
            if (settingsObj != null)
            {
                // Clone the Button
                GameObject mainMenuButton = UnityEngine.Object.Instantiate(settingsObj.gameObject,
                    parentOfParent
                        ? settingsObj.transform.parent.parent
                        : settingsObj.transform.parent);
                if (!parentOfParent)
                    mainMenuButton.transform.SetSiblingIndex(settingsObj.transform.GetSiblingIndex() + 1);

                // Set Clone Position
                Vector3 clonePos = mainMenuButton.transform.localPosition;
                clonePos.x = 0f;
                clonePos.y = -500f;
                clonePos.z = 0f;
                mainMenuButton.transform.localPosition = clonePos;

                // Set Clone Text
                mainMenuButton.name = "ModSettings";
                TextMeshProUGUI cloneSettingsText = mainMenuButton.GetComponentInChildren<TextMeshProUGUI>();
                if (cloneSettingsText != null)
                    cloneSettingsText.text = $"Mod Settings";
                LocalizedText cloneLocalized = mainMenuButton.GetComponentInChildren<LocalizedText>();
                if (cloneLocalized != null)
                    UnityEngine.Object.Destroy(cloneLocalized);

                // Set Original Text
                settingsObj.name = "GameSettings";
                TextMeshProUGUI settingsText = settingsObj.GetComponentInChildren<TextMeshProUGUI>();
                if (settingsText != null)
                    settingsText.text = $"Game Settings";
                LocalizedText localized = settingsObj.GetComponentInChildren<LocalizedText>();
                if (localized != null)
                    UnityEngine.Object.Destroy(localized);

                // Set Clone Event
                UMUIButton cloneSettingsButton = mainMenuButton.GetComponentInChildren<UMUIButton>();
                if (cloneSettingsButton != null)
                {
                    // Create Event to Open Custom Tab
                    cloneSettingsButton.OnClick = new();
                    cloneSettingsButton.OnClick.AddListener(new Action(() => ModSettingsManager.OpenModSettings()));
                }
            }
        }
    }
}
