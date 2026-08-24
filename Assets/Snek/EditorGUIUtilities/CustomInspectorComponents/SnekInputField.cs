using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekInputField
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly string _label;
        private readonly bool _isInputDelayed;

        private readonly float _fieldWidth;
        private readonly float _fieldHeight;

        private GUIStyle _labelStyle;
        private GUIStyle _fieldStyle;

        private readonly GUILayoutOption[] _options;
        private readonly GUILayoutOption _widthOption;

        /// <summary>
        /// <list type="bullet">fieldWidth = 0f -> expandable width</list>
        /// </summary>
        public SnekInputField(
            SerializedProperty serializedProperty,
            string label,
            bool delayInput,
            float fieldWidth = SnekGUILayout.DefaultFieldWidth,
            float fieldHeight = SnekGUILayout.DefaultFieldHeight,
            params GUILayoutOption[] options)
        {
            _serializedProperty = serializedProperty;
            _label = label;
            _isInputDelayed = delayInput;

            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            _options = options;

            _widthOption = _fieldWidth == 0f ?
                GUILayout.ExpandWidth(true) : GUILayout.Width(_fieldWidth);
        }

        private void InitializeStyles(GUIStyle labelStyle, GUIStyle fieldStyle)
        {
            if (_labelStyle == null)
                _labelStyle = labelStyle == null ? SnekGUIStyles.Label() : labelStyle;

            if (_fieldStyle == null)
                _fieldStyle = fieldStyle == null ? SnekGUIStyles.TextField() : fieldStyle;
        }

        public void DrawHorizontal(GUIStyle labelStyle = null, GUIStyle fieldStyle = null)
        {
            InitializeStyles(labelStyle, fieldStyle);

            using (new SnekPropertyHorizontalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                GUILayout.Label(_label, _labelStyle, GUILayout.Height(_fieldHeight));

                DrawInputField(_widthOption, GUILayout.Height(_fieldHeight));
            }
        }

        public void DrawVertical(GUIStyle labelStyle = null, GUIStyle fieldStyle = null)
        {
            InitializeStyles(labelStyle, fieldStyle);

            using (new SnekPropertyVerticalScope(_serializedProperty, SnekGUIScopeOption.SetGUILayoutOptions(_options)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    GUILayout.Label(_label, _labelStyle, GUILayout.Height(_fieldHeight));

                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    DrawInputField(_widthOption, GUILayout.Height(_fieldHeight));
            }
        }

        private void DrawInputField(params GUILayoutOption[] options)
        {
            switch (_serializedProperty.propertyType)
            {
                case SerializedPropertyType.Integer:

                    if (_isInputDelayed)
                        SnekGUILayout.DrawDelayedIntField(_serializedProperty, _fieldStyle, options);
                    else
                        SnekGUILayout.DrawIntField(_serializedProperty, _fieldStyle, options);

                    break;

                case SerializedPropertyType.Float:

                    if (_isInputDelayed)
                        SnekGUILayout.DrawDelayedFloatField(_serializedProperty, _fieldStyle, options);
                    else
                        SnekGUILayout.DrawFloatField(_serializedProperty, _fieldStyle, options);

                    break;

                case SerializedPropertyType.String:

                    if (_isInputDelayed)
                        SnekGUILayout.DrawDelayedStringField(_serializedProperty, _fieldStyle, options);
                    else
                        SnekGUILayout.DrawStringField(_serializedProperty, _fieldStyle, options);

                    break;

                default:

                    Debug.LogError("Invalid value type property used for Snek Input Field, cannot draw.");

                    break;
            }
        }
    }
}
