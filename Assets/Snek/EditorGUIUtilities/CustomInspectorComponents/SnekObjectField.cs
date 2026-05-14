using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekObjectField<T> where T : Object
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly string _label;
        private readonly bool _allowSceneObjects;

        private GUIStyle _labelStyle;

        private readonly GUILayoutOption[] _options;

        public SnekObjectField(
            SerializedProperty serializedProperty,
            string label,
            bool allowSceneObjects,
            params GUILayoutOption[] options) 
        {
            _serializedProperty = serializedProperty;
            _label = label;
            _allowSceneObjects = allowSceneObjects;

            _options = options;
        }

        private void InitializeLabelStyle(GUIStyle labelStyle)
        {
            if (_labelStyle == null)
                _labelStyle = labelStyle == null ? SnekGUIStyles.Label() : labelStyle;
        }

        public void DrawHorizontal(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyHorizontalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                GUILayout.Label(_label, _labelStyle);
                SnekGUILayout.DrawObjectField<T>(_serializedProperty, _allowSceneObjects, _options);
            }
        }

        public void DrawVertical(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyVerticalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    GUILayout.Label(_label, _labelStyle);

                SnekGUILayout.DrawObjectField<T>(_serializedProperty, _allowSceneObjects, _options);
            }
        }
    }
}
