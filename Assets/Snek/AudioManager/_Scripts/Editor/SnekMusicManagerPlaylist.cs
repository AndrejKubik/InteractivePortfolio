using SnekEditor.GUIUtilities;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

namespace SnekEditor.AudioManager
{
    public class SnekMusicManagerPlaylist : SnekReorderableList
    {
        private readonly SerializedObject so_MusicManager;
        private readonly SerializedProperty sp_DefaultTrack;
        private readonly SerializedProperty sp_RandomDefaultTrack;

        private bool _isMouseOverMusicManager = false;
        private bool _isMouseOverPlaylist = false;

        private readonly SnekDropBox _trackDropBox;

        public SnekMusicManagerPlaylist(
            SerializedObject serializedObject,
            SerializedProperty tracksList,
            SerializedProperty defaultTrack,
            SerializedProperty randomDefaultTrack)
            : base(serializedObject, tracksList, false)
        {
            so_MusicManager = serializedObject;
            sp_DefaultTrack = defaultTrack;
            sp_RandomDefaultTrack = randomDefaultTrack;

            _trackDropBox = new SnekDropBox(OnDragAndDrop);
        }

        private bool IsTrackSetAsDefault(int index)
        {
            return !IsEditingMultipleManagersWithDifferentDefaultTracks()
                && sp_RandomDefaultTrack.boolValue == false
                && index == sp_DefaultTrack.intValue;
        }

        private bool IsDefaultTrackAssigningAllowed()
        {
            return !sp_RandomDefaultTrack.hasMultipleDifferentValues && sp_RandomDefaultTrack.boolValue == false;
        }

        private bool IsEditingMultipleManagersWithDifferentDefaultTracks()
        {
            return so_MusicManager.isEditingMultipleObjects && sp_DefaultTrack.hasMultipleDifferentValues;
        }

        private bool IsDraggingObjectsOverPlaylist()
        {
            return SnekUnityEditorUtility.IsDragAndDropActive() && _isMouseOverPlaylist;
        }

        private bool IsDraggingObjectsOverMusicManager()
        {
            return SnekUnityEditorUtility.IsDragAndDropActive() && _isMouseOverMusicManager;
        }

        protected override bool IsElementPropertyInteractionAllowed()
        {
            return !IsDraggingObjectsOverPlaylist();
        }

        protected override string GetHeaderLabel()
        {
            return $"Playlist ({serializedProperty.arraySize})";
        }

        protected override string GetElementLabel(int index)
        {
            return $"Track {index}";
        }

        protected override void OnReorderElements(ReorderableList list, int oldIndex, int newIndex)
        {
            int defaultTrackIndex = sp_DefaultTrack.intValue;

            if (defaultTrackIndex == oldIndex)
                defaultTrackIndex = newIndex;
            else if (newIndex > oldIndex) //moved an element downwards
            {
                if (oldIndex < defaultTrackIndex && defaultTrackIndex <= newIndex)
                    defaultTrackIndex--;
            }
            else if (newIndex < oldIndex) //moved an element upwards
            {
                if (newIndex <= defaultTrackIndex && defaultTrackIndex < oldIndex)
                    defaultTrackIndex++;
            }

            if (defaultTrackIndex != sp_DefaultTrack.intValue)
            {
                sp_DefaultTrack.intValue = defaultTrackIndex;
                so_MusicManager.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public void SetIsMouseOverMusicManager(bool newState)
        {
            _isMouseOverMusicManager = newState;
        }

        private bool InitializeIcons()
        {
            return _iconsContainer != null
                && _iconsContainer.GetFavourite(false) != null
                && _iconsContainer.GetFavourite(true) != null
                && _iconsContainer.GetFoldout(true) != null
                && _iconsContainer.GetFoldout(false) != null;
        }



        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var splitter = new SnekRectSplitter(rect);

            if (IsDefaultTrackAssigningAllowed())
            {
                Rect defaultToggleRect = splitter.TakeLeft(rect.height);

                DrawDefaultTrackToggle(defaultToggleRect, index);
            }

            Rect propertyRect = splitter.TakeRemaining();

            base.DrawElement(propertyRect, index, isActive, isFocused);
        }

        public override void Draw()
        {
            if (!InitializeIcons())
            {
                GUILayout.Label("Failed to find required textures, cannot draw list.");

                return;
            }

            base.Draw();

            Rect listRect = GUILayoutUtility.GetLastRect();

            if (Event.current.type != EventType.Layout)
                _isMouseOverPlaylist = SnekGUIUtility.IsCursorOverRect(listRect);

            if (IsDraggingObjectsOverMusicManager())
                _trackDropBox.Draw(listRect, _headerStyle);
        }

        private void DrawDefaultTrackToggle(Rect rect, int index)
        {
            if (IsTrackSetAsDefault(index))
            {
                SnekGUILayout.DrawTexture(rect, _iconsContainer.GetFavourite(true), GUI.color);

                return;
            }

            SerializedProperty listElement = serializedProperty.GetArrayElementAtIndex(index);

            if (listElement.objectReferenceValue == null)
                return;

            bool buttonClicked = GUI.Button(rect, _iconsContainer.GetFavourite(false), EditorStyles.label);

            if (buttonClicked)
            {
                sp_DefaultTrack.intValue = index;

                serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            bool isHovered = rect.Contains(Event.current.mousePosition);

            if (isHovered)
                SnekGUILayout.DrawTextureTransparent(rect, _iconsContainer.GetFavourite(true), alpha: 0.5f);
        }



        private void OnDragAndDrop(Object[] droppedObjects, string[] droppedAssetPaths)
        {
            foreach (Object droppedObject in droppedObjects)
                if (droppedObject is AudioClip audioClip && !SnekEditorUtility.IsObjectInArray(audioClip, serializedProperty))
                    AddAudioClipToPlaylist(audioClip);

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private void AddAudioClipToPlaylist(AudioClip audioClip)
        {
            serializedProperty.arraySize++;

            SerializedProperty newElement = serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1);
            newElement.objectReferenceValue = audioClip;
        }
    }
}
