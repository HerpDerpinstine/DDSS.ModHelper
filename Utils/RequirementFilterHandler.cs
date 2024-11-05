using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DDSS_ModHelper.Utils
{
    public static class RequirementFilterHandler
    {
        private static List<MelonBase> _optionalMelons = new();

        public static void AddOptionalMelon(MelonBase melon)
        {
            if (!_optionalMelons.Contains(melon))
                _optionalMelons.Add(melon);
        }

        [Serializable]
        internal class SerializedRequirement
        {
            internal SerializedRequirement(string id,
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

        internal static string Generate()
        {
            List<SerializedRequirement> serializedRequirements = new();

            // Add Plugins
            foreach (var plugin in MelonPlugin.RegisteredMelons)
                if (!_optionalMelons.Contains(plugin))
                    serializedRequirements.Add(new(string.IsNullOrEmpty(plugin.ID)
                        ? plugin.Info.Name : plugin.ID,
                        plugin.Info.Name,
                        plugin.Info.Author,
                        plugin.Info.Version));

            // Add Mods
            foreach (var mod in MelonMod.RegisteredMelons)
                if (!_optionalMelons.Contains(mod))
                    serializedRequirements.Add(new(string.IsNullOrEmpty(mod.ID)
                        ? mod.Info.Name : mod.ID,
                        mod.Info.Name,
                        mod.Info.Author,
                        mod.Info.Version));

            // Callback

            return JsonConvert.SerializeObject(serializedRequirements);
        }

        internal static void Parse(string json,
            out List<(SerializedRequirement, MelonBase)> missingMods,
            out List<(SerializedRequirement, MelonBase)> mismatchedMods)
        {
            List<SerializedRequirement> serializedRequirements = JsonConvert.DeserializeObject<List<SerializedRequirement>>(json);

            missingMods = new();
            mismatchedMods = new();

            foreach (SerializedRequirement requirement in serializedRequirements)
            {
                MelonBase melonFound = MelonBase.FindMelon(requirement.Name, requirement.Author);
                if (melonFound == null)
                {
                    // Callback

                    missingMods.Add((requirement, null));
                    continue;
                }
                if (melonFound.Info.Version != requirement.Version)
                    mismatchedMods.Add((requirement, melonFound));
            }
        }
    }
}