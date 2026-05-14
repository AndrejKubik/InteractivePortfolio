using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekOverrideTextFieldStyleScope : IDisposable
    {
        private readonly TextAnchor _alignment;
        private readonly int _fontSize;
        private readonly FontStyle _fontStyle;

        public SnekOverrideTextFieldStyleScope()
        {
            _alignment = EditorStyles.textField.alignment;
            _fontSize = EditorStyles.textField.fontSize;
            _fontStyle = EditorStyles.textField.fontStyle;

            GUIStyle snekStyle = SnekGUIStyles.TextField();

            EditorStyles.textField.alignment = snekStyle.alignment;
            EditorStyles.textField.fontSize = snekStyle.fontSize;
            EditorStyles.textField.fontStyle = snekStyle.fontStyle;
        }

        public void Dispose()
        {
            EditorStyles.textField.alignment = _alignment;
            EditorStyles.textField.fontSize = _fontSize;
            EditorStyles.textField.fontStyle = _fontStyle;
        }
    }
}
