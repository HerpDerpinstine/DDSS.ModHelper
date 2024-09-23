﻿using Il2Cpp;
using Il2CppUI.Tabs.SettingsTab;
using System;
using UnityEngine;

namespace DDSS_ModHelper.Settings.Internal
{
    internal static class ModSettingsOptionBuilder
    {
        internal static string _keyCodePrefix = "KeyCode_";
        internal static int _keyCodePrefixLen = _keyCodePrefix.Length;

        private static SettingObject CreateObject(Setting setting)
        {
            GameObject obj = UnityEngine.Object.Instantiate(ModSettingsManager._tab.settingPrefab,
                ModSettingsManager._tab.settingsParent);
            SettingObject settingObj = obj.GetComponent<SettingObject>();
            settingObj.SetSetting(setting, GameSettingsManager.instance, true, true);
            return settingObj;
        }

        private static Setting CreateSetting(string name,
            string description,
            SettingType type = SettingType.MultiChoice)
        {
            Setting setting = new();
            setting.devOnly = false;
            setting.Key = name;
            setting.presetName = string.Empty;
            setting.type = type;
            setting.additionalInfo = description;
            return setting;
        }

        internal static SettingObject CreateKeybind(string name,
            string description,
            KeyCode value)
        {
            Setting setting = CreateSetting(name, description, SettingType.KeyBind);
            setting.axisName = $"{_keyCodePrefix}{Enum.GetName(typeof(KeyCode), value)}";
            SettingObject obj = CreateObject(setting);
            obj.keyBind.RefreshKeyBindText();
            return obj;
        }

        internal static SettingObject CreateToggle(string name,
            string description,
            bool value)
        {
            Setting setting = CreateSetting(name, description);
            setting.alternatives = new();
            setting.alternatives.Add("OFF");
            setting.alternatives.Add("ON");
            setting.Value = value ? 1f : 0f;
            return CreateObject(setting);
        }

        internal static SettingObject CreateNumber<T>(
            string name,
            string description,
            T value,
            T minValue,
            T maxValue)
            where T : IConvertible
        {
            Setting setting = CreateSetting(name, description);
            setting.axisName = "MODDED";
            setting.alternatives = new();
            setting.alternatives.Add(typeof(float).FullName);
            setting.alternatives.Add(minValue.ToString());
            setting.alternatives.Add(maxValue.ToString());

            if (typeof(T) == typeof(float))
                setting.Value = (float)Math.Round(Convert.ToDouble(value), 1, MidpointRounding.AwayFromZero);
            else
                setting.Value = Convert.ToSingle(value);

            return CreateObject(setting);
        }

        internal static SettingObject CreateEnum(string name,
            string description,
            string value,
            Type enumType)
        {
            Setting setting = CreateSetting(name, description);
            setting.alternatives = new();

            string[] valueNames = Enum.GetNames(enumType);
            foreach (string valueName in valueNames)
                setting.alternatives.Add(valueName);

            int valueIndex = 0;
            Array allValues = Enum.GetValues(enumType);
            for (int i = 0; i < allValues.Length; i++)
            {
                if (allValues.GetValue(i).ToString() != value)
                    continue;
                valueIndex = i;
                break;
            }
            setting.Value = valueIndex;

            return CreateObject(setting);
        }
    }
}
