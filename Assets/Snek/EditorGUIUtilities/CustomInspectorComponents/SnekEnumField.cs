using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekEnumField
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly string _label;
        private readonly string[] _valueNames;

        private GUIStyle _labelStyle;

        private readonly GUILayoutOption[] _options;

        public SnekEnumField(
            SerializedProperty serializedProperty,
            string label,
            string[] valueNames,
            params GUILayoutOption[] options)
        {
            _serializedProperty = serializedProperty;
            _label = label;
            _valueNames = valueNames;

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

            if (labelStyle == null)
                labelStyle = _labelStyle;

            using (new SnekPropertyHorizontalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                GUILayout.Label(_label, _labelStyle);

                using (new SnekGUIVerticalScope(SnekGUIScopeAnchor.Center))
                    SnekGUILayout.DrawEnumField(_serializedProperty, _valueNames);
            }
        }

        public void DrawVertical(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyVerticalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    GUILayout.Label(_label, _labelStyle);

                SnekGUILayout.DrawEnumField(_serializedProperty, _valueNames);
            }
        }
    }
}
