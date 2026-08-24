using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekColorField
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly string _label;

        private readonly float _fieldWidth;
        private readonly float _fieldHeight;

        private GUIStyle _labelStyle;

        private readonly GUILayoutOption[] _options;
        private readonly GUILayoutOption _widthOption;

        /// <summary>
        /// <list type="bullet">fieldWidth = 0f -> expandable width</list>
        /// </summary>
        public SnekColorField(
            SerializedProperty serializedProperty,
            string label,
            float fieldWidth = SnekGUILayout.DefaultFieldWidth,
            float fieldHeight = SnekGUILayout.DefaultFieldHeight,
            params GUILayoutOption[] options)
        {
            _serializedProperty = serializedProperty;
            _label = label;

            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            _options = options;

            _widthOption = _fieldWidth == 0f ?
                GUILayout.ExpandWidth(true) : GUILayout.Width(_fieldWidth);
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
                GUILayout.Label(_label, _labelStyle, GUILayout.Height(_fieldHeight));
                DrawColorField(_widthOption, GUILayout.Height(_fieldHeight));
            }
        }

        public void DrawVertical(GUIStyle labelStyle = null)
        {
            InitializeLabelStyle(labelStyle);

            using (new SnekPropertyVerticalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    GUILayout.Label(_label, _labelStyle, GUILayout.Height(_fieldHeight));

                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    DrawColorField(_widthOption, GUILayout.Height(_fieldHeight));
            }
        }

        private void DrawColorField(params GUILayoutOption[] options)
        {
            if(_serializedProperty.propertyType == SerializedPropertyType.Color)
                SnekGUILayout.DrawColorField(_serializedProperty, options);
            else
                Debug.LogError("Invalid value type property used for Snek Color Field, cannot draw.");
        }
    }
}
