using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekPropertyHorizontalScope : IDisposable
    {
        private SnekPropertyScope _propertyScope;

        public Rect GetScopeRect()
        {
            return _propertyScope.GetScopeRect();
        }

        public SnekPropertyHorizontalScope(SerializedProperty property, params SnekGUIScopeOption[] options)
        {
            _propertyScope = new SnekPropertyScope(property, SnekGUIDrawDirection.Horizontal, options);
        }

        public void Dispose()
        {
            _propertyScope.Dispose();
        }
    }
}
