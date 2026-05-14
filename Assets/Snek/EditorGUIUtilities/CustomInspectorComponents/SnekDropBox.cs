using SnekEditor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekDropBox
    {
        public delegate void DragAndDropCallback(Object[] droppedObjects, string[] droppedAssetPaths);

        private readonly DragAndDropCallback _onDropCallback;
        private readonly string _dropMessage;

        private readonly Color _backgroundOverlayColor;
        private readonly Color _backgroundOverlayHighlightColor;
        private readonly Color _borderColor;
        private readonly Color _dropHighlightOverlayColor;

        private GUIStyle _dropMessageStyle;
        private GUIStyle _headerStyle;

        public SnekDropBox(
            DragAndDropCallback onDropCallback,
            string dropMessage = "Drop it like it's hot!",
            Color? backgroundOverlayColor = null,
            Color? backgroundOverlayHighlightColor = null,
            Color? borderColor = null,
            Color? dropHighlightOverlayColor = null)
        {
            _onDropCallback = onDropCallback;
            _dropMessage = dropMessage;

            _backgroundOverlayColor = backgroundOverlayColor.HasValue ?
                backgroundOverlayColor.Value : Color.black;

            _backgroundOverlayHighlightColor = backgroundOverlayHighlightColor.HasValue ?
                backgroundOverlayHighlightColor.Value : Color.white;

            _borderColor = borderColor.HasValue ?
                borderColor.Value : Color.white;

            _dropHighlightOverlayColor = dropHighlightOverlayColor.HasValue ?
                dropHighlightOverlayColor.Value : Color.white;
        }

        private void InitializeHeaderStyle()
        {
            if (_headerStyle == null)
                _headerStyle = SnekGUIStyles.BoldLabel();
        }

        private void InitializeDropMessageStyle(GUIStyle dropMessageStyle)
        {
            if (_dropMessageStyle != null)
                return;

            if (dropMessageStyle != null)
            {
                _dropMessageStyle = dropMessageStyle;

                return;
            }

            _dropMessageStyle = SnekGUIStyles.BoldLabel(
                stretchWidth: true,
                stretchHeight: true,
                wordWrap: true);
        }

        public void DrawStandalone(string label, GUIStyle dropMessageStyle = null, params GUILayoutOption[] options)
        {
            InitializeHeaderStyle();

            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                GUILayout.Label(label, _headerStyle);

            GUILayout.Label(GUIContent.none, options);
            Rect rect = GUILayoutUtility.GetLastRect();

            Draw(rect, dropMessageStyle);
        }

        public void Draw(Rect rect, GUIStyle dropMessageStyle = null)
        {
            InitializeDropMessageStyle(dropMessageStyle);

            SnekGUILayout.DrawRectTransparent(rect, _backgroundOverlayColor, 0.75f);
            SnekGUILayout.DrawRectTransparent(rect, _backgroundOverlayHighlightColor, 0.2f);
            SnekGUILayout.DrawColoredBorder(rect, _borderColor, 1f);

            GUI.Label(rect, _dropMessage, _dropMessageStyle);

            if (!SnekGUIUtility.IsCursorOverRect(rect) || !SnekUnityEditorUtility.IsDragAndDropActive())
                return;

            SnekGUILayout.DrawRectTransparent(rect, _dropHighlightOverlayColor);

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (Event.current.type == EventType.DragPerform)
                _onDropCallback.Invoke(DragAndDrop.objectReferences, DragAndDrop.paths);
        }
    }
}
