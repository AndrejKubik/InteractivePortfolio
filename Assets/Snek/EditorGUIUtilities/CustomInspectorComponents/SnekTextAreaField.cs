using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekTextAreaField
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly string _label;

        private readonly float _fieldWidth;
        private readonly float _fieldMinHeight;

        private GUIStyle _labelStyle;
        private GUIStyle _fieldStyle;

        private readonly GUILayoutOption[] _options;
        private readonly GUILayoutOption _widthOption;

        /// <summary>
        /// <list type="bullet">fieldWidth = 0f -> expandable width</list>
        /// </summary>
        public SnekTextAreaField(
            SerializedProperty serializedProperty,
            string label,
            float fieldWidth = SnekGUILayout.DefaultFieldWidth,
            float fieldMinHeight = SnekGUILayout.DefaultFieldHeight,
            params GUILayoutOption[] options)
        {
            _serializedProperty = serializedProperty;
            _label = label;

            _fieldWidth = fieldWidth;
            _fieldMinHeight = fieldMinHeight;

            _options = options;

            _widthOption = _fieldWidth == 0f ?
                GUILayout.ExpandWidth(true) : GUILayout.Width(_fieldWidth);
        }

        private void InitializeLabelStyle(GUIStyle labelStyle)
        {
            if (_labelStyle == null)
                _labelStyle = labelStyle == null ? SnekGUIStyles.Label() : labelStyle;

            if (_fieldStyle == null)
                _fieldStyle = EditorStyles.textArea;
        }

        public void DrawHorizontal(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyHorizontalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                GUILayout.Label(_label, _labelStyle, GUILayout.Height(_fieldMinHeight));

                DrawInputField(_widthOption, GUILayout.Height(GetFieldHeight()));
            }
        }

        public void DrawVertical(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyVerticalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    GUILayout.Label(_label, _labelStyle, GUILayout.Height(_fieldMinHeight));

                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    DrawInputField(_widthOption, GUILayout.Height(GetFieldHeight()));
            }
        }

        private float GetFieldHeight()
        {
            return EditorGUI.GetPropertyHeight(_serializedProperty, true);
        }

        private void DrawInputField(params GUILayoutOption[] options)
        {
            if (_serializedProperty.propertyType == SerializedPropertyType.String)
                SnekGUILayout.DrawTextAreaField(_serializedProperty, _fieldStyle, options);
            else
                Debug.LogError("Invalid value type property used for Snek Text Area Field, cannot draw.");
        }
    }
}
