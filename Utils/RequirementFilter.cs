using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DDSS_ModHelper.Utils
{
    public static class RequirementFilter
    {
        private static List<MelonBase> _optionalMelons = new();
        private static Dictionary<string, SerializedRequirement> _additionalRequirements = new();

        private static List<SerializedRequirement> _generatedList = new();

        public static void AddOptionalMelon(MelonBase melon)
        {
            if (!_optionalMelons.Contains(melon))
                _optionalMelons.Add(melon);
        }

        public static void AddRequirement(SerializedRequirement requirement)
        {
            string key = string.IsNullOrEmpty(requirement.ID) ? requirement.Name : requirement.ID;
            if (!_additionalRequirements.ContainsKey(key))
                _additionalRequirements[key] = requirement;
        }

        [Serializable]
        public class SerializedRequirement
        {
            public SerializedRequirement(string id,
                string name,
                string author,
                string version)
            {
                ID = id;
                Name = name;
                Author = author;
                Version = version;
            }

            public readonly string ID;
            public readonly string Name;
            public readonly string Author;
            public readonly string Version;
        }

        internal static string Generate()
        {
            // Clear Generated List
            _generatedList.Clear();

            // Add Plugins
            foreach (var plugin in MelonPlugin.RegisteredMelons)
                if (!_optionalMelons.Contains(plugin))
                    _generatedList.Add(new(string.IsNullOrEmpty(plugin.ID)
                        ? plugin.Info.Name : plugin.ID,
                        plugin.Info.Name,
                        plugin.Info.Author,
                        plugin.Info.Version));

            // Add Mods
            foreach (var mod in MelonMod.RegisteredMelons)
                if (!_optionalMelons.Contains(mod))
                    _generatedList.Add(new(string.IsNullOrEmpty(mod.ID)
                        ? mod.Info.Name : mod.ID,
                        mod.Info.Name,
                        mod.Info.Author,
                        mod.Info.Version));

            // Add Additional Requirements
            _generatedList.AddRange(_additionalRequirements.Values);

            // Return JSON
            return JsonConvert.SerializeObject(_generatedList);
        }

        internal static void Parse(string json,
            out List<SerializedRequirement> missingMods,
            out List<SerializedRequirement> mismatchedMods)
        {
            List<SerializedRequirement> serializedRequirements = JsonConvert.DeserializeObject<List<SerializedRequirement>>(json);

            missingMods = new();
            mismatchedMods = new();

            foreach (SerializedRequirement requirement in serializedRequirements)
            {
                string key = string.IsNullOrEmpty(requirement.ID) ? requirement.Name : requirement.ID;
                if (_additionalRequirements.ContainsKey(key))
                {
                    SerializedRequirement found = _additionalRequirements[key];
                    if (found.Version != requirement.Version)
                        mismatchedMods.Add(requirement);
                    continue;
                }
                else
                {
                    MelonBase melonFound = MelonBase.FindMelon(requirement.Name, requirement.Author);
                    if (melonFound == null)
                    {
                        missingMods.Add(requirement);
                        continue;
                    }
                    if (melonFound.Info.Version != requirement.Version)
                        mismatchedMods.Add(requirement);
                }
            }
        }
    }
}