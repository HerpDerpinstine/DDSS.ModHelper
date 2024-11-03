using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DDSS_ModHelper.Utils
{
    public static class ModFilterHandler
    {
        private static List<MelonBase> _optionalMelons = new();

        public static void AddOptionalMelon<T>(T melon) where T : MelonBase
        {
            if (!_optionalMelons.Contains(melon))
                _optionalMelons.Add(melon);
        }

        [Serializable]
        internal class SerializedMod
        {
            internal SerializedMod(string id,
                string name,
                string author,
                string version)
            {
                ID = id;
                Name = name;
                Author = author;
                Version = version;
            }

            internal string ID;
            internal string Name;
            internal string Author;
            internal string Version;
        }

        internal static string GenerateModRequirementList()
        {
            List<SerializedMod> serializedMods = new();

            foreach (var plugin in MelonPlugin.RegisteredMelons)
                if (!_optionalMelons.Contains(plugin))
                    serializedMods.Add(new(string.IsNullOrEmpty(plugin.ID)
                        ? plugin.Info.Name : plugin.ID,
                        plugin.Info.Name,
                        plugin.Info.Author,
                        plugin.Info.Version));

            foreach (var mod in MelonMod.RegisteredMelons)
                if (!_optionalMelons.Contains(mod))
                    serializedMods.Add(new(string.IsNullOrEmpty(mod.ID)
                        ? mod.Info.Name : mod.ID,
                        mod.Info.Name,
                        mod.Info.Author,
                        mod.Info.Version));

            return JsonConvert.SerializeObject(serializedMods);
        }

        internal static void ParseModRequirements(string json,
            out List<(SerializedMod, MelonBase)> missingMods,
            out List<(SerializedMod, MelonBase)> mismatchedMods)
        {
            List<SerializedMod> serializedMods = JsonConvert.DeserializeObject<List<SerializedMod>>(json);

            missingMods = new();
            mismatchedMods = new();

            foreach (SerializedMod mod in serializedMods)
            {
                MelonBase melonFound = MelonBase.FindMelon(mod.Name, mod.Author);
                if (melonFound == null)
                {
                    missingMods.Add((mod, null));
                    continue;
                }
                if (melonFound.Info.Version != mod.Version)
                    mismatchedMods.Add((mod, melonFound));
            }
        }
    }
}