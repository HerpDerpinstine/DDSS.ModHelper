using Il2CppUMUI;
using UnityEngine;

namespace DDSS_ModHelper.Utils
{
    internal static class AlternateStyleHandler
    {
        internal enum eBackgroundStyle
        {
            Gradient = 0,
            Flat = 1
        }

        internal enum eLogoStyle
        {
            Transparent = 0,
            Opaque = 1
        }

        internal static void Apply()
        {
            Transform backgroundFlatMenu = UIManager.instance.transform.Find("Background");
            if (backgroundFlatMenu != null)
            {
                eBackgroundStyle menuBackgroundStyle = ConfigHandler._prefs_MenuBackgroundStyle.Value;
                backgroundFlatMenu.gameObject.SetActive(menuBackgroundStyle == eBackgroundStyle.Flat);
            }
            else
            {
                eBackgroundStyle lobbyBackgroundStyle = ConfigHandler._prefs_LobbyBackgroundStyle.Value;
                Transform backgroundFlatLobby = UIManager.instance.transform.Find("Background (1)");
                if (backgroundFlatLobby != null)
                    backgroundFlatLobby.gameObject.SetActive(lobbyBackgroundStyle == eBackgroundStyle.Flat);
            }

            eLogoStyle menuLogoStyle = ConfigHandler._prefs_MenuLogoStyle.Value;

            Transform logoOpaque = UIManager.instance.transform.Find("MenuTab/Tab/Tasks/TopBar/Logo (1)");
            if (logoOpaque != null)
                logoOpaque.gameObject.SetActive(menuLogoStyle == eLogoStyle.Opaque);

            Transform logoTransparent = UIManager.instance.transform.Find("MenuTab/Tab/Tasks/TopBar/Logo");
            if (logoTransparent != null)
                logoTransparent.gameObject.SetActive(menuLogoStyle == eLogoStyle.Transparent);
        }
    }
}
