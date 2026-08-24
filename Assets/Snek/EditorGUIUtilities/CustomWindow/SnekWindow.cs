using System;
using SnekEditor.ScriptableObjectManager;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekWindow : EditorWindow
    {
        protected SnekWindowGUILayoutSettings _layoutSettings;

        private bool _isValid = true;
        private bool _isInitialized = false;

        private void OnEnable()
        {
            _layoutSettings = SnekScriptableObjectManager.GetAsset<SnekWindowGUILayoutSettings>();

            if (!_layoutSettings)
            {
                Debug.LogError($"Cannot find requred {nameof(SnekWindowGUILayoutSettings)} asset, closing window...");
                Close();
            }

            _isInitialized = false;
        }

        private void OnGUI()
        {
            if (!_isInitialized)
                Initialize();

            Validate();

            if (!_isValid)
            {
                Debug.LogError("Closing window to avoid issues.");
                Close();

                return;
            }

            if (!_isInitialized)
            {
                OnInitializationSuccess();

                _isInitialized = true;
            }

            if (IsReinitializationRequired())
            {
                _isValid = true;
                _isInitialized = false;

                OnBeforeReinitialize();
            }

            SnekGUILayout.DrawRect(GetEffectiveWindowRect(), GetBackgroundColor());
            SnekGUILayout.DrawColoredBorder(GetEffectiveWindowRect(), GetBorderColor(), GetBorderWidth());

            using (new SnekGUIHorizontalScope())
            {
                GUILayout.Space(GetBorderWidth());

                using (new SnekGUIVerticalScope())
                {
                    GUILayout.Space(GetBorderWidth());

                    if (!IsReinitializationRequired()) //must check here again instead of returning before to preserve GUI scope structure and avoid console errors
                        DrawContent();

                    GUILayout.Space(GetBorderWidth());
                }

                GUILayout.Space(GetBorderWidth());
            }
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void Validate()
        {

        }

        protected virtual void OnInitializationSuccess()
        {

        }

        protected void FailValidation(string errorMessage)
        {
            Debug.LogError(errorMessage);

            _isValid = false;
        }

        protected virtual bool IsReinitializationRequired()
        {
            return false;
        }

        protected virtual void OnBeforeReinitialize()
        {

        }

        private Rect GetEffectiveWindowRect()
        {
            return new Rect(position)
            {
                position = Vector2.zero
            };
        }

        protected virtual Color GetBackgroundColor()
        {
            return _layoutSettings.GetBackgroundColor();
        }

        protected virtual Color GetContentColor()
        {
            return _layoutSettings.GetContentColor();
        }

        protected virtual Color GetBorderColor()
        {
            return _layoutSettings.GetBorderColor();
        }

        protected virtual float GetBorderWidth()
        {
            return _layoutSettings.GetBorderWidth();
        }

        protected virtual void DrawContent()
        {

        }
    }
}
