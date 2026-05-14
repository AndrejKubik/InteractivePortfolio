using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public static class SnekGUIUtility
    {
        public static GUIStyle GetStyleWithPadding(GUIStyle style, RectOffset padding)
        {
            return new GUIStyle(style)
            {
                padding = padding
            };
        }

        public static RectOffset GetUniformPadding(int padding)
        {
            return new RectOffset(padding, padding, padding, padding);
        }

        public static RectOffset Get2DPadding(int horizontalPadding, int verticalPadding)
        {
            return new RectOffset(horizontalPadding, horizontalPadding, verticalPadding, verticalPadding);
        }

        /// <summary>
        /// Helper method in case you want to store the options into a variable before creating a gui scope instance for reusability
        /// </summary>
        public static GUILayoutOption[] CreateLayoutOptions(params GUILayoutOption[] options)
        {
            return options;
        }

        /// <summary>
        /// <list type="bullet">Note: In case you are using this for Rects generated through GUILayoutUtility.GetLastRect(),
        /// make sure to put this check under following:</list>
        /// <c>if (Event.current.type != EventType.Layout)</c>
        /// </summary>
        public static bool IsCursorOverRect(Rect rect)
        {
            return rect.Contains(Event.current.mousePosition);
        }

        public static void ShowEditorAlert(string title, string message, Action onCloseCallback = null)
        {
            var alertWindow = EditorWindow.CreateInstance<SnekEditorAlert>();

            alertWindow.titleContent = new GUIContent(title);
            alertWindow.Message = message;
            alertWindow.OnCloseCallback = onCloseCallback;

            alertWindow.IsOpenedCorrectly = true;

            SnekEditorAlertManager.ShowAlert(alertWindow);
        }
    }
}
