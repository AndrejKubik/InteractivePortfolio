using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekBoolField
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly string _label;

        private GUIStyle _labelStyle;

        private readonly GUILayoutOption[] _options;

        public SnekBoolField(
            SerializedProperty serializedProperty,
            string label,
            params GUILayoutOption[] options)
        {
            _serializedProperty = serializedProperty;
            _label = label;

            _options = options;
        }

        private void InitializeLabelStyle(GUIStyle labelStyle)
        {
            if(_labelStyle == null)
                _labelStyle = labelStyle == null ? SnekGUIStyles.Label() : labelStyle;
        }

        public void Draw(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyHorizontalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                GUILayout.Label(_label, _labelStyle, GUILayout.ExpandHeight(true));

                SnekGUILayout.DrawBoolField(
                    _serializedProperty,
                    GUILayout.Width(SnekGUILayout.UnityCheckboxWidth),
                    GUILayout.ExpandHeight(true));
            }
        }
    }
}
