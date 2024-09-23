using DDSS_ModHelper.Components;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace DDSS_ModHelper.Utils
{
    public static class ModHelperExtensions
    {
        private static Regex _rtRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static string RemoveRichText(this string val)
            => _rtRegex.Replace(val, string.Empty);

        public static int ColorFloatToInt(this float val)
            => Mathf.Max(0, Mathf.Min(255, (int)Mathf.Floor(val * 256)));

        public static System.Drawing.Color ToDrawingColor(this Color color)
            => System.Drawing.Color.FromArgb(
                color.a.ColorFloatToInt(),
                color.r.ColorFloatToInt(),
                color.g.ColorFloatToInt(),
                color.b.ColorFloatToInt());

        public static Coroutine StartCoroutine<T>(this T behaviour, IEnumerator enumerator)
            where T : MonoBehaviour
            => behaviour.StartCoroutine(
                new Il2CppSystem.Collections.IEnumerator(
                new ManagedEnumerator(enumerator).Pointer));

        public static void AddListener(this UnityEvent action,
            Action listener)
            => action.AddListener(listener);
        public static void AddListener<T>(this UnityEvent<T> action,
            Action<T> listener)
            => action.AddListener(listener);

        public static void RemoveListener(this UnityEvent action,
            Action listener)
            => action.RemoveListener(listener);
        public static void RemoveListener<T>(this UnityEvent<T> action,
            Action<T> listener)
            => action.RemoveListener(listener);
    }
}
