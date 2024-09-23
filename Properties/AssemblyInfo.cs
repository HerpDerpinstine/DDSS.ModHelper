using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(DDSS_ModHelper.Properties.BuildInfo.Description)]
[assembly: AssemblyDescription(DDSS_ModHelper.Properties.BuildInfo.Description)]
[assembly: AssemblyCompany(DDSS_ModHelper.Properties.BuildInfo.Company)]
[assembly: AssemblyProduct(DDSS_ModHelper.Properties.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + DDSS_ModHelper.Properties.BuildInfo.Author)]
[assembly: AssemblyTrademark(DDSS_ModHelper.Properties.BuildInfo.Company)]
[assembly: AssemblyVersion(DDSS_ModHelper.Properties.BuildInfo.Version)]
[assembly: AssemblyFileVersion(DDSS_ModHelper.Properties.BuildInfo.Version)]
[assembly: MelonInfo(typeof(DDSS_ModHelper.MelonMain), 
    DDSS_ModHelper.Properties.BuildInfo.Name, 
    DDSS_ModHelper.Properties.BuildInfo.Version,
    DDSS_ModHelper.Properties.BuildInfo.Author,
    DDSS_ModHelper.Properties.BuildInfo.DownloadLink)]
[assembly: MelonGame("StripedPandaStudios", "DDSS")]
[assembly: MelonPriority(int.MinValue)]
[assembly: VerifyLoaderVersion("0.6.5", true)]
[assembly: HarmonyDontPatchAll]