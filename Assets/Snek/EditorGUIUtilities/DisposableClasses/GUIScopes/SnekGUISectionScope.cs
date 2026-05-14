using System;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekGUISectionScope : IDisposable
    {
        private readonly SnekGUIVerticalScope _guiScope;

        public Rect GetRect()
        {
            return _guiScope.GetRect();
        }

        public SnekGUISectionScope(string headerText, GUIStyle headerStyle = null, GUIStyle scopeStyle = null, params GUILayoutOption[] options)
        {
            _guiScope = new SnekGUIVerticalScope(
                SnekGUIScopeAnchor.NoAnchor,
                SnekGUIScopeOption.SetStyle(scopeStyle),
                SnekGUIScopeOption.SetGUILayoutOptions(options));

            DrawHeader(new GUIContent(headerText), headerStyle);
        }

        public SnekGUISectionScope(GUIContent header, GUIStyle headerStyle = null, GUIStyle scopeStyle = null, params GUILayoutOption[] options)
        {
            _guiScope = new SnekGUIVerticalScope(
                SnekGUIScopeAnchor.NoAnchor,
                SnekGUIScopeOption.SetStyle(scopeStyle),
                SnekGUIScopeOption.SetGUILayoutOptions(options));

            if (header != GUIContent.none)
                DrawHeader(header, headerStyle);
        }

        public void Dispose()
        {
            _guiScope.Dispose();
        }

        private void DrawHeader(GUIContent headerContent, GUIStyle headerStyle)
        {
            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                GUILayout.Label(headerContent, headerStyle);

            GUILayout.Space(10f);
        }
    }
}
