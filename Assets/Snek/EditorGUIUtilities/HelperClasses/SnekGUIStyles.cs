using UnityEngine;
using UnityEditor;

namespace SnekEditor.GUIUtilities
{
    public static class SnekGUIStyles
    {
        public static GUIStyle TextureButton(int padding = 2)
        {
            return new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(padding, padding, padding, padding)
            };
        }

        public static GUIStyle BoldTextButton(int fontSize = 12, bool stretchWidth = true, bool stretchHeight = true)
        {
            return new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = stretchWidth,
                stretchHeight = stretchHeight
            };
        }

        public static GUIStyle TextButton(int fontSize = 12, bool stretchWidth = true, bool stretchHeight = true)
        {
            return new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSize,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = stretchWidth,
                stretchHeight = stretchHeight
            };
        }

        public static GUIStyle BoldLabel(
            int fontSize = 12,
            TextAnchor anchor = TextAnchor.MiddleCenter,
            bool stretchWidth = false,
            bool stretchHeight = false,
            int padding = 0,
            bool wordWrap = false)
        {
            return new GUIStyle(EditorStyles.whiteLabel)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = anchor,
                stretchWidth = stretchWidth,
                stretchHeight = stretchHeight,
                padding = new RectOffset(padding, padding, padding, padding),
                wordWrap = wordWrap
            };
        }

        public static GUIStyle Label(
            int fontSize = 12,
            TextAnchor anchor = TextAnchor.MiddleCenter,
            bool stretchWidth = false,
            bool stretchHeight = false,
            int padding = 0,
            bool wordWrap = false)
        {
            return new GUIStyle(EditorStyles.whiteLabel)
            {
                fontSize = fontSize,
                alignment = anchor,
                stretchWidth = stretchWidth,
                stretchHeight = stretchHeight,
                padding = new RectOffset(padding, padding, padding, padding),
                wordWrap = wordWrap
            };
        }

        public static GUIStyle TextField(int fontSize = 12, bool stretchWidth = true, bool stretchHeight = true)
        {
            return new GUIStyle(EditorStyles.textField)
            {
                fontSize = fontSize,
                alignment = TextAnchor.MiddleCenter,
                stretchHeight = stretchHeight,
                stretchWidth = stretchWidth,
                wordWrap = false
            };
        }

        public static GUIStyle HelpBox(
            int fontSize = 12, 
            TextAnchor anchor = TextAnchor.MiddleLeft, 
            bool stretchWidth = true,
            bool stretchHeight = true,
            bool wordWrap = true,
            int padding = 10)
        {
            return new GUIStyle(EditorStyles.whiteLabel)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = anchor,
                stretchWidth = stretchWidth,
                stretchHeight = stretchHeight,
                wordWrap = wordWrap,
                padding = new RectOffset(padding, padding, padding, padding)
            };
        }

        public static GUIStyle SectionTitle(
            int fontSize = 20,
            TextAnchor anchor = TextAnchor.MiddleCenter,
            bool stretchWidth = true,
            bool stretchHeight = true,
            int padding = 10)
        {
            return new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = anchor,
                stretchWidth = stretchWidth,
                stretchHeight = stretchHeight,
                wordWrap = false,
                padding = new RectOffset(padding, padding, padding, padding)
            };
        }

        public static GUIStyle SectionScope(RectOffset padding)
        {
            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            return new GUIStyle(EditorStyles.helpBox)
            {
                stretchWidth = true,
                stretchHeight = true,
                padding = padding,
                margin = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(0, 0, 0, 0)
            };
        }
    }
}
