using System;
using Snek.Utilities;
using SnekEditor.ScriptableObjectManager;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekAdjustableSlider
    {
        private readonly SerializedProperty sp_CurrentValue;
        private readonly SerializedProperty sp_MinValue;
        private readonly SerializedProperty sp_MaxValue;
        private readonly SerializedProperty sp_UseWholeNumbers;

        private readonly string _label;

        private readonly float _fieldWidth;
        private readonly float _fieldHeight;
        private readonly GUILayoutOption[] _options;

        private GUIStyle _sectionStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _alertStyle;

        private GUIStyle _labelStyle;
        private GUIStyle _fieldStyle;

        private SnekInspectorGUILayoutSettings _visualSettings;

        private readonly SnekInputField _minValueField;
        private readonly SnekInputField _maxValueField;
        private readonly SnekInputField _defaultValueField;

        private readonly SnekBoolField _useWholeNumbersField;

        public SnekAdjustableSlider(
            SerializedProperty snekSliderDataProperty,
            string label,
            float fieldWidth = SnekGUILayout.DefaultFieldWidth,
            float fieldHeight = SnekGUILayout.DefaultFieldHeight,
            params GUILayoutOption[] options)
        {
            sp_CurrentValue = snekSliderDataProperty.FindPropertyRelative(nameof(SnekSliderData.CurrentValue));
            sp_MinValue = snekSliderDataProperty.FindPropertyRelative(nameof(SnekSliderData.MinValue));
            sp_MaxValue = snekSliderDataProperty.FindPropertyRelative(nameof(SnekSliderData.MaxValue));
            sp_UseWholeNumbers = snekSliderDataProperty.FindPropertyRelative(nameof(SnekSliderData.UseWholeNumbers));

            _label = label;

            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            _options = options;

            _minValueField = new SnekInputField(
                sp_MinValue,
                "Min",
                _fieldWidth,
                _fieldHeight,
                GUILayout.Width(_fieldWidth));

            _maxValueField = new SnekInputField(
                sp_MaxValue,
                "Max",
                _fieldWidth,
                _fieldHeight,
                GUILayout.Width(_fieldWidth));

            _defaultValueField = new SnekInputField(
                sp_CurrentValue,
                "Value:",
                _fieldWidth,
                _fieldHeight);

            _useWholeNumbersField = new SnekBoolField(
                sp_UseWholeNumbers,
                "Use Whole Numbers");
        }

        private bool InitializeVisualSettings()
        {
            if (_visualSettings == null)
                _visualSettings = SnekScriptableObjectManager.GetAsset<SnekInspectorGUILayoutSettings>();

            return _visualSettings != null;
        }

        private void InitializeStyles(GUIStyle labelStyle, GUIStyle fieldStyle)
        {
            if (_sectionStyle == null)
                _sectionStyle = SnekGUIStyles.SectionScope(_visualSettings.GetSectionPadding());

            if (_headerStyle == null)
                _headerStyle = SnekGUIStyles.BoldLabel(14);

            if (_labelStyle == null)
                _labelStyle = labelStyle == null ?
                    SnekGUIStyles.Label(stretchWidth: true, stretchHeight: true) : labelStyle;

            if (_fieldStyle == null)
                _fieldStyle = fieldStyle == null ? SnekGUIStyles.TextField() : fieldStyle;

            if (_alertStyle == null)
                _alertStyle = SnekGUIStyles.HelpBox();
        }

        public void Draw(GUIStyle labelStyle = null, GUIStyle fieldStyle = null)
        {
            if (!InitializeVisualSettings())
            {
                GUILayout.Label("Failed to find SnekInspectorGUILayoutSettings asset, cannot draw list.");

                return;
            }

            InitializeStyles(labelStyle, fieldStyle);

            using (new SnekGUISectionScope(_label, _headerStyle, _sectionStyle))
            {
                using (new SnekGUIHorizontalScope(_options))
                {
                    using (new EditorGUI.DisabledScope(sp_UseWholeNumbers.hasMultipleDifferentValues))
                        DrawMinValueField();

                    GUILayout.Space(10f);

                    using (new SnekGUIVerticalScope())
                    {
                        using (new EditorGUI.DisabledScope(sp_UseWholeNumbers.hasMultipleDifferentValues))
                            DrawCurrentValueFieldAndSlider();
                    }

                    GUILayout.Space(10f);

                    using (new EditorGUI.DisabledScope(sp_UseWholeNumbers.hasMultipleDifferentValues))
                        DrawMaxValueField();
                }

                DrawUseWholeNumbersField();

                if (IsChangesBlockedAlertRequired())
                {
                    GUILayout.Space(20f);
                    SnekGUILayout.DrawAlertMessage("Some changes blocked due to mixed values.", _alertStyle);
                }
            }
        }

        private void DrawUseWholeNumbersField()
        {
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    _useWholeNumbersField.Draw();

                if (changeScope.changed)
                    sp_CurrentValue.floatValue = Mathf.Round(sp_CurrentValue.floatValue);
            }
        }

        private void DrawCurrentValueFieldAndSlider()
        {
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUI.DisabledScope(sp_MinValue.hasMultipleDifferentValues || sp_MaxValue.hasMultipleDifferentValues))
                {
                    using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                        _defaultValueField.DrawHorizontal(_labelStyle, _fieldStyle);

                    SnekGUILayout.DrawHorizontalSlider(sp_CurrentValue, sp_MinValue.floatValue, sp_MaxValue.floatValue);
                }

                if (changeScope.changed)
                    KeepDefaultValueWithinRange();
            }
        }

        private void DrawMinValueField()
        {
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                _minValueField.DrawVertical(_labelStyle, _fieldStyle);

                if (changeScope.changed)
                {
                    sp_MinValue.floatValue = Mathf.Min(sp_MinValue.floatValue, sp_MaxValue.floatValue);

                    KeepDefaultValueWithinRange();
                }
            }
        }

        private void DrawMaxValueField()
        {
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                _maxValueField.DrawVertical(_labelStyle, _fieldStyle);

                if (changeScope.changed)
                {
                    sp_MaxValue.floatValue = Mathf.Max(sp_MinValue.floatValue, sp_MaxValue.floatValue);

                    KeepDefaultValueWithinRange();
                }
            }
        }

        private bool IsChangesBlockedAlertRequired()
        {
            return sp_MinValue.hasMultipleDifferentValues
                || sp_MaxValue.hasMultipleDifferentValues
                || sp_UseWholeNumbers.hasMultipleDifferentValues;
        }

        private void KeepDefaultValueWithinRange()
        {
            float newValue = Mathf.Clamp(sp_CurrentValue.floatValue, sp_MinValue.floatValue, sp_MaxValue.floatValue);

            sp_CurrentValue.floatValue = sp_UseWholeNumbers.boolValue == true ?
                Mathf.Round(newValue) : MathF.Round(newValue, 2);
        }
    }
}
