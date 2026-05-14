using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekPropertyVerticalScope : IDisposable
    {
        private SnekPropertyScope _propertyScope;

        public Rect GetScopeRect()
        {
            return _propertyScope.GetScopeRect();
        }

        public SnekPropertyVerticalScope(SerializedProperty property, params SnekGUIScopeOption[] options)
        {
            _propertyScope = new SnekPropertyScope(property, SnekGUIDrawDirection.Vertical, options);
        }

        public void Dispose()
        {
            _propertyScope.Dispose();
        }
    }
}
