using Il2Cpp;
using Il2CppLocalization;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DDSS_ModHelper.Settings.Internal
{
    internal static class ModSettingsPanelBuilder
    {
        internal static void Create(ref SettingsTab tab, 
            ref RectTransform tabRect)
        {
            if (tab != null
               && !tab.WasCollected)
                return;

            // Find Settings Tab
            SettingsTab[] comps = Resources.FindObjectsOfTypeAll<SettingsTab>();
            foreach (SettingsTab comp in comps)
            {
                // Clone it for Mod Settings
                GameObject _settingsTabObj = UnityEngine.Object.Instantiate(comp.gameObject, comp.transform.parent);
                tab = _settingsTabObj.GetComponent<SettingsTab>();
                tabRect = tab.settingsParent.GetComponent<RectTransform>();
                GridLayoutGroup tabLayout = tab.settingsParent.GetComponent<GridLayoutGroup>();
                tabLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                tabLayout.constraintCount = 1;
                ScaleContentToGridLayout scale = tab.settingsParent.gameObject.AddComponent<ScaleContentToGridLayout>();
                scale.contentRectTransform = tabRect;
                scale.gridLayoutGroup = tabLayout;
                _settingsTabObj.name = tab.name = "ModSettings";
                tab.InitTab();
                tab.scrollRect.verticalNormalizedPosition = 1f;
                tab.scrollRect.verticalScrollbar.value = 1f;
                tab.scrollRect.SetDirty();

                // Move and Stretch Scroll View
                Vector2 scrollViewOffset = tab.scrollRect.rectTransform.offsetMax;
                scrollViewOffset.y += 46f;
                tab.scrollRect.rectTransform.offsetMax = scrollViewOffset;

                // Move and Stretch Viewport
                Vector2 viewportOffset = tab.scrollRect.viewRect.offsetMax;
                viewportOffset.y += 44f;
                tab.scrollRect.viewRect.offsetMax = viewportOffset;

                // Change Original Title
                Transform titleTrans = comp.transform.Find("Tab/Tasks/TopBar/Title");
                if (titleTrans != null)
                {
                    TextMeshProUGUI settingsText = titleTrans.GetComponentInChildren<TextMeshProUGUI>();
                    if (settingsText != null)
                        settingsText.text = $"Game Settings";
                    LocalizedText localized = titleTrans.GetComponentInChildren<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                // Change Clone Title
                Transform cloneTitleTrans = _settingsTabObj.transform.Find("Tab/Tasks/TopBar/Title");
                if (cloneTitleTrans != null)
                {
                    TextMeshProUGUI cloneSettingsText = cloneTitleTrans.GetComponentInChildren<TextMeshProUGUI>();
                    if (cloneSettingsText != null)
                        cloneSettingsText.text = $"Mod Settings";
                    LocalizedText localized = cloneTitleTrans.GetComponentInChildren<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                // Remove Extra Categories Buttons
                Transform categoriesGrid = _settingsTabObj.transform.Find("Tab/Tasks/CatagoriesGrid");
                if (categoriesGrid != null)
                    GameObject.Destroy(categoriesGrid.gameObject);

                // Move and Stretch Scroll View Background
                Transform backgroundTrans = _settingsTabObj.transform.Find("Tab/Tasks/Background");
                if (cloneTitleTrans != null)
                {
                    RectTransform rectTrans = backgroundTrans.GetComponent<RectTransform>();
                    Vector2 rectOffset = rectTrans.offsetMax;
                    rectOffset.y += 44f;
                    rectTrans.offsetMax = rectOffset;
                }

                break;
            }
        }
    }
}
