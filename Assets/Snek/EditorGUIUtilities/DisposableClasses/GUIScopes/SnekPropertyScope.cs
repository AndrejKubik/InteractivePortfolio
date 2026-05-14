using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekPropertyScope : IDisposable
    {
        private readonly SnekGUIScope _guiScope;

        public Rect GetScopeRect()
        {
            return _guiScope.Rect;
        }

        public SnekPropertyScope(SerializedProperty property, SnekGUIDrawDirection direction, params SnekGUIScopeOption[] options)
        {
            _guiScope = new SnekGUIScope(direction, SnekGUIScopeAnchor.NoAnchor, options);

            EditorGUI.BeginProperty(_guiScope.Rect, GUIContent.none, property);
        }

        public void Dispose()
        {
            EditorGUI.EndProperty();

            _guiScope.Dispose();
        }
    }
}
