using Snek.GameUI;
using Snek.Utilities;
using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SnekEditor.GameUI
{
    [CustomEditor(typeof(SnekUISlider), true), CanEditMultipleObjects]
    public class SnekUISliderInspector : SnekMonoBehaviourInspectorCustom<SnekUISlider>
    {
        private const float DragThresholdSettingsWidth = 200f;
        private const float DragAreaSettingsWidth = 185f;

        protected GUIStyle _labelStyle;
        protected GUIStyle _alertStyle;

        private SerializedProperty sp_SetupData;

        private SerializedProperty sp_UseDragThreshold;
        private SerializedProperty sp_DragThresholdPercent;

        private SerializedProperty sp_UseDragAreas;
        private SerializedProperty sp_DragAreaCount;

        private SnekAdjustableSlider _slider;

        private SnekInputField _dragThresholdPercentField;
        private SnekInputField _dragAreaCountField;

        private SnekBoolField _useDragThresholdField;
        private SnekBoolField _useDragAreasField;

        protected override void OnCreateInspectorInstance()
        {
            sp_SetupData = GetProperty(nameof(SnekUISlider.SetupData), true);

            sp_UseDragThreshold = GetProperty(nameof(SnekUISlider.UseDragThreshold), true);
            sp_DragThresholdPercent = GetProperty(nameof(SnekUISlider.DragThresholdPercent), true);

            sp_UseDragAreas = GetProperty(nameof(SnekUISlider.UseDragAreas), true);
            sp_DragAreaCount = GetProperty(nameof(SnekUISlider.DragAreaCount), true);

            _slider = new SnekAdjustableSlider(sp_SetupData, "Range Setup");
            _dragThresholdPercentField = new SnekInputField(sp_DragThresholdPercent, "Drag Threshold (%)");
            _dragAreaCountField = new SnekInputField(sp_DragAreaCount, "Drag Areas Count");

            _useDragThresholdField = new SnekBoolField(sp_UseDragThreshold, "Use Drag Threshold");
            _useDragAreasField = new SnekBoolField(sp_UseDragAreas, "Use Drag Areas");
        }

        protected override bool Initialize()
        {
            return InitializeAlertStyle()
                && InitializeLabelStyle()
                && base.Initialize();
        }

        private bool InitializeLabelStyle()
        {
            if (_labelStyle == null)
                _labelStyle = SnekGUIStyles.Label();

            return _labelStyle != null;
        }

        private bool InitializeAlertStyle()
        {
            if (_alertStyle == null)
                _alertStyle = SnekGUIStyles.HelpBox();

            return _alertStyle != null;
        }

        protected override void OnPropertiesChange()
        {
            foreach (SnekUISlider component in GetSelectedComponents())
                if (component.enabled)
                    ApplySetupDataToActualSlider(component);
        }

        protected void ApplySetupDataToActualSlider(SnekUISlider component)
        {
            Slider slider = component.GetComponent<Slider>();
            SnekSliderData setupData = component.SetupData;

            using (new SnekDirectValueChangeScope(slider))
            {
                slider.wholeNumbers = setupData.UseWholeNumbers;
                slider.minValue = setupData.MinValue;
                slider.maxValue = setupData.MaxValue;
                slider.value = setupData.CurrentValue;
            }
        }

        protected override void DrawContent()
        {
            DrawSliderSetupGUI();

            GUILayout.Space(5f);

            DrawBaseProperties();
        }

        protected void DrawSliderSetupGUI()
        {
            _slider.Draw();

            GUILayout.Space(5f);

            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
            {
                DrawDragThresholdSettings();

                GUILayout.FlexibleSpace();

                DrawDragAreaSettings();
            }
        }

        private void DrawDragThresholdSettings()
        {
            using (new SnekGUISectionScope(GUIContent.none, _sectionHeaderStyle, _sectionStyle, GUILayout.Width(DragThresholdSettingsWidth)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    _useDragThresholdField.Draw();

                if (IsDragThresholdPercentFieldRequired())
                    using (new EditorGUI.DisabledScope(sp_UseDragThreshold.hasMultipleDifferentValues))
                    {
                        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                            _dragThresholdPercentField.DrawHorizontal();

                        if (sp_UseDragThreshold.hasMultipleDifferentValues)
                            SnekGUILayout.DrawAlertMessage("Changes blocked due to mixed values", _alertStyle);
                    }
            }
        }

        private bool IsDragThresholdPercentFieldRequired()
        {
            return sp_UseDragThreshold.hasMultipleDifferentValues || sp_UseDragThreshold.boolValue == true;
        }

        private void DrawDragAreaSettings()
        {
            using (new SnekGUISectionScope(GUIContent.none, _sectionHeaderStyle, _sectionStyle, GUILayout.Width(DragAreaSettingsWidth)))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                    _useDragAreasField.Draw();

                if (IsDragAreaCountFieldRequired())
                    using (new EditorGUI.DisabledScope(sp_UseDragAreas.hasMultipleDifferentValues))
                    {
                        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                            _dragAreaCountField.DrawHorizontal();

                        if (sp_UseDragThreshold.hasMultipleDifferentValues)
                            SnekGUILayout.DrawAlertMessage("Changes blocked due to mixed values", _alertStyle);
                    }
            }
        }

        private bool IsDragAreaCountFieldRequired()
        {
            return sp_UseDragAreas.hasMultipleDifferentValues || sp_UseDragAreas.boolValue == true;
        }
    }
}
