using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekPropertyFieldScope : IDisposable
    {
        private EditorGUI.MixedValueScope _mixedValueScope;
        private EditorGUI.ChangeCheckScope _changeCheckScope;

        public SnekPropertyFieldScope(SerializedProperty property)
        {
            _mixedValueScope = new EditorGUI.MixedValueScope(property.hasMultipleDifferentValues);
            _changeCheckScope = new EditorGUI.ChangeCheckScope();
        }

        public void Dispose()
        {
            _changeCheckScope.Dispose();

            var mixedValueScope = _mixedValueScope as IDisposable;
            mixedValueScope.Dispose();
        }

        public bool IsValueChanged()
        {
            return _changeCheckScope.changed;
        }
    }
}
