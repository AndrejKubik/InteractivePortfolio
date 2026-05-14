using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.PlayModeManager
{
    public class SnekPlayModeManagerWindow : SnekWindow
    {
        private const float FeatureDisabledStateBorderWidth = 3f;

        private const float WindowMinWidth = 475f;
        private const float WindowMinHeight = 600f;

        private const float FeatureToggleStateWidth = 100f;
        private const float FeatureToggleHeight = 50f;

        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _alertStyle;

        private SnekPlayModeManagerConfig _config;

        protected override void OnCreateWindowInstance()
        {
            Undo.undoRedoPerformed += OnUndoRedo;

            titleContent = new GUIContent("Play Mode Start Scene Manager");
            minSize = new Vector2(WindowMinWidth, WindowMinHeight);

            SnekPlayModeManager.InitializeFiles();

            _config = SnekPlayModeManager.GetConfigFile();

            if (_config.IsFeatureEnabled())
                PreventInvalidDefaultScene();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            Repaint();
            PreventInvalidDefaultScene();
        }

        protected override Color GetBorderColor()
        {
            return _config.IsFeatureEnabled() ?
                base.GetBorderColor() : Color.gray;
        }

        protected override float GetBorderWidth()
        {
            return _config.IsFeatureEnabled() ?
                base.GetBorderWidth() : FeatureDisabledStateBorderWidth;
        }

        private void PreventInvalidDefaultScene()
        {
            if (!IsDefaultSceneAssigned())
                using (new SnekDirectValueChangeScope(_config))
                    _config.EnableFeature(false);
        }

        private bool IsDefaultSceneAssigned()
        {
            return _config.GetDefaultScene() != null;
        }

        private void InitializeStyles()
        {
            if (_titleStyle == null)
                _titleStyle = SnekGUIStyles.SectionTitle(35);

            if (_labelStyle == null)
                _labelStyle = SnekGUIStyles.BoldLabel(16, stretchHeight: true);

            if (_buttonStyle == null)
                _buttonStyle = SnekGUIStyles.BoldTextButton(20);

            if (_alertStyle == null)
                _alertStyle = SnekGUIStyles.HelpBox(anchor: TextAnchor.MiddleCenter);
        }



        protected override void DrawContent()
        {
            InitializeStyles();

            GUILayout.Space(50f);

            DrawTitle();

            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
            {
                using (new SnekGUIVerticalScope())
                {
                    GUILayout.FlexibleSpace();

                    DrawDefaultSceneSelection();

                    using (new EditorGUI.DisabledGroupScope(!IsDefaultSceneAssigned()))
                        DrawFeatureToggle();

                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void DrawTitle()
        {
            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
            {
                string titleText =
                    "Play Mode\n" +
                    "Start Scene Manager";

                GUILayout.Box(titleText, _titleStyle);

                if (_config.IsFeatureEnabled())
                {
                    Rect rect = GUILayoutUtility.GetLastRect();

                    using (new SnekGUIColoredScope(GetContentColor()))
                        GUI.Box(rect, titleText, _titleStyle);
                }
            }
        }

        private void DrawDefaultSceneSelection()
        {
            using (new SnekGUIVerticalScope(SnekGUIScopeAnchor.Center))
            {
                using (new SnekGUIHorizontalScope())
                {
                    GUILayout.Label("Default Scene", _labelStyle);

                    GUILayout.Space(5f);

                    DrawDefaultSceneField();
                }

                if (!IsDefaultSceneAssigned())
                    DrawNoSceneAssignedAlert();
            }
        }

        private void DrawNoSceneAssignedAlert()
        {
            GUILayout.Label("No default scene assigned!", _alertStyle);
        }

        private void DrawDefaultSceneField()
        {
            var newDefaultScene = EditorGUILayout.ObjectField(
                _config.GetDefaultScene(),
                typeof(SceneAsset),
                false,
                GUILayout.Height(50f))
                as SceneAsset;

            if (newDefaultScene != _config.GetDefaultScene())
            {
                using (new SnekDirectValueChangeScope(_config, "Change Play Mode Default Scene"))
                    _config.SetDefaultScene(newDefaultScene);

                PreventInvalidDefaultScene();
            }
        }

        private void DrawFeatureToggle()
        {
            var scope = new SnekGUIHorizontalScope(SnekGUIScopeAnchor.NoAnchor, GUILayout.Height(FeatureToggleHeight));

            using (scope)
            {
                GUILayout.Box("Enforce Default Scene", _labelStyle);

                DrawEnabledState();
                DrawDisabledState();
            }
        }

        private void DrawEnabledState()
        {
            Rect rect = GUILayoutUtility.GetRect(
                GUIContent.none,
                _buttonStyle,
                GUILayout.Width(FeatureToggleStateWidth));

            using (new SnekGUIColoredScope(GetContentColor()))
            {
                if (!_config.IsFeatureEnabled())
                    DrawEnableButton(rect);
                else
                {
                    GUI.Box(rect, "ON", _titleStyle);
                    SnekGUILayout.DrawColoredBorder(rect, GetBorderColor(), FeatureDisabledStateBorderWidth);
                }
            }
        }

        private void DrawEnableButton(Rect rect)
        {
            bool buttonClicked = GUI.Button(rect, "Enable", _buttonStyle);

            if (buttonClicked)
                using (new SnekDirectValueChangeScope(_config))
                    _config.EnableFeature(true);
        }

        private void DrawDisabledState()
        {
            Rect rect = GUILayoutUtility.GetRect(
                GUIContent.none,
                _buttonStyle,
                GUILayout.Width(FeatureToggleStateWidth));

            if (_config.IsFeatureEnabled())
                DrawDisableButton(rect);
            else
            {
                GUI.Box(rect, "OFF", _titleStyle);
                SnekGUILayout.DrawColoredBorder(rect, Color.gray, FeatureDisabledStateBorderWidth);
            }
        }

        private void DrawDisableButton(Rect rect)
        {
            bool buttonClicked = GUI.Button(rect, "Disable", _buttonStyle);

            if (buttonClicked)
                using (new SnekDirectValueChangeScope(_config))
                    _config.EnableFeature(false);
        }
    }
}