using SnekEditor.ScriptableObjectManager;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekWindow : EditorWindow
    {
        protected SnekWindowGUILayoutSettings _layoutSettings;

        private void OnEnable()
        {
            _layoutSettings = SnekScriptableObjectManager.GetAsset<SnekWindowGUILayoutSettings>();

            if (!_layoutSettings)
            {
                Debug.LogError($"Cannot find requred {nameof(SnekWindowGUILayoutSettings)} asset, closing window...");
                Close();
            }

            OnCreateWindowInstance();
        }

        private void OnGUI()
        {
            SnekGUILayout.DrawRect(GetEffectiveWindowRect(), GetBackgroundColor());
            SnekGUILayout.DrawColoredBorder(GetEffectiveWindowRect(), GetBorderColor(), GetBorderWidth());

            DrawContent();
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

        protected virtual void OnCreateWindowInstance()
        {

        }

        protected virtual void DrawContent()
        {

        }
    }
}
