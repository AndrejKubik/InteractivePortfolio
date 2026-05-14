using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SnekEditor.GUIUtilities;
using Snek.AudioManager;

namespace SnekEditor.AudioManager
{
    [CustomEditor(typeof(SnekMusicManager)), CanEditMultipleObjects]
    public class SnekMusicManagerInspector : SnekMonoBehaviourInspectorCustom<SnekMusicManager>
    {
        private GUIStyle _alertStyle;
        private GUIStyle _labelStyle;

        private SerializedProperty sp_MusicTracks;
        private SerializedProperty sp_RandomDefaultTrack;
        private SerializedProperty sp_DefaultTrack;
        private SerializedProperty sp_FadeBetweenTracks;
        private SerializedProperty sp_FadeTransitionDuration;
        private SerializedProperty sp_ShuffleTracks;

        private SnekMusicManagerPlaylist list_MusicTracks;

        private SnekInputField _fadeTransitionDurationField;

        private SnekBoolField _shuffleTracksField;
        private SnekBoolField _randomDefaultTrackField;
        private SnekBoolField _fadeBetweenTracksField;

        protected override void OnCreateInspectorInstance()
        {
            sp_MusicTracks = serializedObject.FindProperty(nameof(SnekMusicManager.MusicTracks));
            sp_RandomDefaultTrack = serializedObject.FindProperty(nameof(SnekMusicManager.RandomDefaultTrack));
            sp_DefaultTrack = serializedObject.FindProperty(nameof(SnekMusicManager.DefaultTrack));
            sp_FadeBetweenTracks = serializedObject.FindProperty(nameof(SnekMusicManager.FadeBetweenTracks));
            sp_FadeTransitionDuration = serializedObject.FindProperty(nameof(SnekMusicManager.FadeTransitionDuration));
            sp_ShuffleTracks = serializedObject.FindProperty(nameof(SnekMusicManager.ShuffleTracks));

            list_MusicTracks = new SnekMusicManagerPlaylist(
                serializedObject,
                sp_MusicTracks,
                sp_DefaultTrack,
                sp_RandomDefaultTrack);

            _fadeTransitionDurationField = new SnekInputField(sp_FadeTransitionDuration, "Transition Duration");
            _shuffleTracksField = new SnekBoolField(sp_ShuffleTracks, "Shuffle Playback");
            _randomDefaultTrackField = new SnekBoolField(sp_RandomDefaultTrack, "Random Default Track");
            _fadeBetweenTracksField = new SnekBoolField(sp_FadeBetweenTracks, "Fade Between Tracks");
        }


        protected override bool Initialize()
        {
            return InitializeAlertStyle()
                && InitializeLabelStyle()
                && base.Initialize();
        }

        private bool InitializeAlertStyle()
        {
            if (_alertStyle == null)
                _alertStyle = SnekGUIStyles.HelpBox();

            return _alertStyle != null;
        }

        private bool InitializeLabelStyle()
        {
            if (_labelStyle == null)
                _labelStyle = SnekGUIStyles.Label();

            return _labelStyle != null;
        }

        protected override void OnPropertiesChange()
        {
            RemoveMusicTrackDuplicates();
            LockDefaultTrackWithinRange();
        }

        private void RemoveMusicTrackDuplicates()
        {
            var noDuplicatesCollection = new HashSet<Object>();

            for (int i = sp_MusicTracks.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty element = sp_MusicTracks.GetArrayElementAtIndex(i);
                Object obj = element.objectReferenceValue;

                if (!noDuplicatesCollection.Add(obj))
                    sp_MusicTracks.DeleteArrayElementAtIndex(i);
            }

            if (serializedObject.ApplyModifiedPropertiesWithoutUndo())
                Debug.Log("Duplicate music track detected and removed from the list.");
        }

        private void LockDefaultTrackWithinRange()
        {
            int maxIndex = Mathf.Max(0, sp_MusicTracks.arraySize - 1); //prevent the index value of -1 when list is empty

            sp_DefaultTrack.intValue = Mathf.Clamp(sp_DefaultTrack.intValue, 0, maxIndex);

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private bool IsEntireSelectionUsingFadeTransition()
        {
            foreach (SnekMusicManager component in GetSelectedComponents())
            {
                if (!component.FadeBetweenTracks)
                    return false;
            }

            return true;
        }



        protected override void DrawContent()
        {
            list_MusicTracks.Draw();
            list_MusicTracks.SetIsMouseOverMusicManager(_isMouseOverComponent);

            if (sp_MusicTracks.arraySize < 1)
            {
                GUILayout.Space(10f);

                SnekGUILayout.DrawAlertMessage("No music tracks assigned.", _alertStyle);

                return;
            }

            GUILayout.Space(10f);

            DrawFadeTransitionSettings();

            GUILayout.Space(10f);

            DrawPlaybackSettings();
        }

        private void DrawFadeTransitionSettings()
        {
            using (new SnekGUISectionScope(GUIContent.none, _sectionHeaderStyle, _sectionStyle))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                {
                    _fadeBetweenTracksField.Draw();

                    if (IsEntireSelectionUsingFadeTransition())
                    {
                        GUILayout.Space(5f);

                        _fadeTransitionDurationField.DrawHorizontal();
                    }
                }
            }
        }

        private void DrawPlaybackSettings()
        {
            using (new SnekGUISectionScope(GUIContent.none, _sectionHeaderStyle, _sectionStyle))
            {
                using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                {
                    using (new SnekGUIVerticalScope(SnekGUIScopeAnchor.Center))
                    {
                        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                            _randomDefaultTrackField.Draw();

                        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                            _shuffleTracksField.Draw();
                    }

                    if (sp_RandomDefaultTrack.boolValue != false || targets.Length != 1 || sp_MusicTracks.arraySize < 1)
                        return;

                    GUILayout.Space(5f);

                    using (new SnekGUIVerticalScope(SnekGUIScopeAnchor.Center))
                    {
                        string defaultTrackName = GetSelectedComponent().MusicTracks[sp_DefaultTrack.intValue].name;

                        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                            GUILayout.Label("Default Track", _labelStyle);

                        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                            GUILayout.Label(defaultTrackName, _sectionHeaderStyle);
                    }
                }
            }
        }
    }
}