using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.PlayModeManager
{
    public static class SnekPlayModeStopper
    {
        private const string PlayModeTintColorValueID = "Playmode tint";

        private static string _userPlayModeTintColorString;
        private static Action _onExitCallback;

        public static void ExitPlayMode(bool playBeep, Action onExitCallback = null)
        {
            if (playBeep)
                EditorApplication.Beep();

            EditorApplication.ExitPlaymode();

            ForceRemovePlayModeTint();

            _onExitCallback = onExitCallback;
            EditorApplication.delayCall = OnPlayModeStop;
        }

        private static void OnPlayModeStop()
        {
            SetPlayModeTintColor(_userPlayModeTintColorString);

            _onExitCallback?.Invoke();
            _onExitCallback = null;
        }

        private static void ForceRemovePlayModeTint()
        {
            _userPlayModeTintColorString = EditorPrefs.GetString(PlayModeTintColorValueID);
            SetPlayModeTintColor(_userPlayModeTintColorString);
        }

        private static void SetPlayModeTintColor(Color color)
        {
            string colorString = $"Playmode tint;{color.r};{color.g};{color.b};{color.a}";

            SetPlayModeTintColor(colorString);
        }

        private static void SetPlayModeTintColor(string colorString)
        {
            EditorPrefs.SetString(PlayModeTintColorValueID, colorString);
        }
    }
}