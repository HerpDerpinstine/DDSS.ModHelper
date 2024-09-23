using Il2CppTMPro;
using UnityEngine;

namespace DDSS_ModHelper.Settings.Internal
{
    internal static class ModSettingsCategoryBuilder
    {
        internal static void Create(string name)
        {
            GameObject catObj = UnityEngine.Object.Instantiate(ModSettingsManager._tab.categoryPrefab, 
                ModSettingsManager._tab.settingsParent);
            TextMeshProUGUI catTmp = catObj.GetComponentInChildren<TextMeshProUGUI>();
            catTmp.text = name;
        }
    }
}
