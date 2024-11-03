using DDSS_ModHelper.Components;
using DDSS_ModHelper.Utils;
using Il2CppTMPro;
using Il2CppUI.Console;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_ModHelper.Console
{
    public static class ConsoleManager
    {
        internal static Dictionary<string, ConsoleCommand> _cmds = new();
        internal static List<string> _cmdHistory = new();
        internal static string _consoleHistory = string.Empty;

        private const int MAX_COMMAND_HISTORY = 100;

        //private const int MAX_CONSOLE_LINES = 10;

        internal static void OnSceneInit()
        {
            ConsoleController[] comps = Resources.FindObjectsOfTypeAll<ConsoleController>();
            foreach (ConsoleController comp in comps)
            {
                comp.gameObject.SetActive(false);

                if ((comp.cheatModeLabel != null)
                    && !comp.cheatModeLabel.WasCollected)
                {
                    TMP_Text txt = comp.cheatModeLabel.GetComponent<TMP_Text>();
                    if ((txt != null)
                        && !txt.WasCollected)
                    {
                        txt.text = "Developer Console Enabled";
                        txt.SetAllDirty();
                    }

                    comp.cheatModeLabel.SetActive(ConfigHandler._prefs_DevConsole.Value);
                }

                comp.enabled = false;

                if (comp.gameObject.GetComponent<CustomConsoleHandler>() == null)
                {
                    CustomConsoleHandler handler = comp.gameObject.AddComponent<CustomConsoleHandler>();
                    handler.enabled = true;
                }

                comp.gameObject.SetActive(true);

                break;
            }
        }

        /*
        private static void EnsureConsoleTextLength()
        {
            string[] array = _consoleHistory.Split('\n', StringSplitOptions.None);
            if (array.Length <= MAX_CONSOLE_LINES)
                return;

            _consoleHistory = string.Empty;
            for (int i = 1; i < array.Length; i++)
            {
                if (i == array.Length - 1)
                    _consoleHistory += array[i];
                else
                    _consoleHistory = _consoleHistory + array[i] + "\n";
            }
        }
        */

        internal static void ApplyConsoleTextToObject()
        {
            //EnsureConsoleTextLength();

            if ((ConsoleController.instance == null)
                || ConsoleController.instance.WasCollected
                || (ConsoleController.instance.console == null)
                || ConsoleController.instance.console.WasCollected)
                return;

            ConsoleController.instance.console.text = _consoleHistory;
            ConsoleController.instance.console.SetAllDirty();
        }

        public static void AddCommand<T>()
            where T : ConsoleCommand, new()
        {
            T cmd = new();
            string name = cmd.GetName();
            if (_cmds.ContainsKey(name))
                throw new Exception($"A Command by the name of {name} already exists!");
            _cmds.Add(name, cmd);
        }

        public static void ExecuteCommand(string fullCmd)
        {
            if (string.IsNullOrEmpty(fullCmd)
                || string.IsNullOrWhiteSpace(fullCmd))
                return;

            if (_cmdHistory.Count >= MAX_COMMAND_HISTORY)
                while (_cmdHistory.Count >= MAX_COMMAND_HISTORY)
                    _cmdHistory.RemoveAt(0);
            _cmdHistory.Add(fullCmd);

            string[] array = fullCmd.Split(' ', 2, StringSplitOptions.None);

            string cmd = array[0];
            if (string.IsNullOrEmpty(cmd)
                || string.IsNullOrWhiteSpace(cmd))
                return;

            if (!_cmds.TryGetValue(cmd, out ConsoleCommand command))
            {
                PrintError($"Command not found: {cmd}");
                return;
            }

            if (array.Length > 1)
            {
                string argsStr = array[1];
                if (string.IsNullOrEmpty(argsStr)
                    || string.IsNullOrWhiteSpace(argsStr))
                    command.Execute(Array.Empty<string>());
                else
                    command.Execute(argsStr.Split(' ', StringSplitOptions.None));
            }
            else
                command.Execute(Array.Empty<string>());
        }

        public static void ExecuteCommand(string cmd, string[] args)
        {
            if (string.IsNullOrEmpty(cmd)
                || string.IsNullOrWhiteSpace(cmd))
                return;

            if (!_cmds.TryGetValue(cmd, out ConsoleCommand command))
            {
                PrintError($"Command not found: {cmd}");
                return;
            }

            PrintMsg($"{cmd} {string.Join(' ', args)}");
            command.Execute(args);
        }

        public static void PrintMsg(string txt)
        {
            MelonMain._logger.Msg($"[CONSOLE]: {txt}");
            _consoleHistory = _consoleHistory + txt + "\n";
            ApplyConsoleTextToObject();
        }

        public static void PrintMsg(Color color, string txt)
        {
            MelonMain._logger.Msg(color.ToDrawingColor(), $"[CONSOLE]: {txt}");
            _consoleHistory = _consoleHistory + "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + txt + "</color>\n";
            ApplyConsoleTextToObject();
        }

        public static void PrintError(string txt)
        {
            MelonMain._logger.Error($"[CONSOLE]: {txt}");
            _consoleHistory = _consoleHistory + "Error : <color=red>" + txt + "</color>\n";
            ApplyConsoleTextToObject();
        }
    }
}
