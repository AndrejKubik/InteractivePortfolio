using Snek.GameUIPlus;
using SnekEditor.GameUI;
using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GameUIPlus
{
    [CustomEditor(typeof(SnekUISliderWithSFX), true), CanEditMultipleObjects]
    public class SnekUISliderWithSFXInspector : SnekUISliderInspector
    {
        private SerializedProperty sp_GrabSound;
        private SerializedProperty sp_DragSound;
        private SerializedProperty sp_ReleaseSound;
        private SerializedProperty sp_SFXCooldown;

        private SnekInputField _sfxCooldownField;

        private SnekObjectField<AudioClip> _grabSoundField;
        private SnekObjectField<AudioClip> _dragSoundField;
        private SnekObjectField<AudioClip> _releaseSoundField;

        protected override void OnCreateInspectorInstance()
        {
            base.OnCreateInspectorInstance();

            sp_GrabSound = GetProperty(nameof(SnekUISliderWithSFX.GrabSound), true);
            sp_DragSound = GetProperty(nameof(SnekUISliderWithSFX.DragSound), true);
            sp_ReleaseSound = GetProperty(nameof(SnekUISliderWithSFX.ReleaseSound), true);
            sp_SFXCooldown = GetProperty(nameof(SnekUISliderWithSFX.SFXCooldown), true);

            _sfxCooldownField = new SnekInputField(sp_SFXCooldown, "SFX Cooldown", 100f);

            _grabSoundField = new SnekObjectField<AudioClip>(sp_GrabSound, "Grab", false);
            _dragSoundField = new SnekObjectField<AudioClip>(sp_DragSound, "Drag", false);
            _releaseSoundField = new SnekObjectField<AudioClip>(sp_ReleaseSound, "Release", false);
        }

        protected override void DrawContent()
        {
            DrawSliderSetupGUI();

            GUILayout.Space(5f);

            DrawSoundSetupGUI();

            GUILayout.Space(5f);

            DrawBaseProperties();
        }

        private void DrawSoundSetupGUI()
        {
            using (new SnekGUISectionScope("Sound Setup", _sectionHeaderStyle, _sectionStyle))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                {
                    DrawAudioClipFields();

                    GUILayout.Space(5f);

                    DrawSFXCooldownField();
                }
            }
        }

        private void DrawAudioClipFields()
        {
            using (new SnekGUIVerticalScope())
            {
                _grabSoundField.DrawHorizontal();
                _dragSoundField.DrawHorizontal();
                _releaseSoundField.DrawHorizontal();
            }
        }

        private void DrawSFXCooldownField()
        {
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                _sfxCooldownField.DrawVertical();

                if (changeScope.changed)
                    sp_SFXCooldown.floatValue = Mathf.Max(0f, sp_SFXCooldown.floatValue);
            }
        }
    }
}
