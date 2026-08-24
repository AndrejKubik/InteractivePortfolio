using System;
using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.AssetBookmarker
{
    public class SnekAssetBookmarkerList : SnekReorderableList
    {
        private const float ElementSpacing = 5f;

        private GUIStyle _textHeaderStyle;
        private GUIStyle _pingButtonStyle;

        private Action<int> _onEditButtonClick;
        private Action<int> _onDeleteButtonClick;

        private Texture2D _editButtonTexture;
        private Texture2D _deleteButtonTexture;

        public SnekAssetBookmarkerList(
            SerializedObject serializedObject,
            SerializedProperty elements,
            Texture2D editButtonTexture,
            Texture2D deleteButtonTexture,
            Action<int> onEditButtonClick,
            Action<int> onDeleteButtonClick)
            : base(serializedObject, elements, false, false, false, false)
        {
            _editButtonTexture = editButtonTexture;
            _deleteButtonTexture = deleteButtonTexture;
            _onEditButtonClick = onEditButtonClick;
            _onDeleteButtonClick = onDeleteButtonClick;

            if (!IsDataValid())
                Debug.LogError("Asset bookmarker list has invalid data, cannot draw.");
        }

        protected override void DrawHeader()
        {
            serializedProperty.isExpanded = true;

            GUILayout.Label(
                "Asset Bookmarks",
                _textHeaderStyle,
                GUILayout.Height(GetHeaderHeight()));
        }

        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SnekSerializedPropertyContext elementContext = GetElementContext(index);

            SerializedProperty sp_Name = elementContext.GetChildProperty(nameof(SnekAssetBookmark.Name));
            SerializedProperty sp_Color = elementContext.GetChildProperty(nameof(SnekAssetBookmark.Color));
            SerializedProperty sp_Tooltip = elementContext.GetChildProperty(nameof(SnekAssetBookmark.Tooltip));
            SerializedProperty sp_Asset = elementContext.GetChildProperty(nameof(SnekAssetBookmark.Asset));

            elementContext.SerializedProperty.isExpanded = false;

            var contentRect = new Rect(rect)
            {
                height = rect.height - 2f * ElementSpacing,
                center = rect.center
            };

            var rectSplitter = new SnekRectSplitter(contentRect);

            Rect deleteButtonRect = rectSplitter.TakeRight(contentRect.height);

            rectSplitter.TakeRight(ElementSpacing);

            Rect editButtonRect = rectSplitter.TakeRight(contentRect.height);
            
            rectSplitter.TakeRight(ElementSpacing);
            
            Rect pingButtonRect = rectSplitter.TakeRemaining();

            DrawPingButton(
                pingButtonRect,
                sp_Name.stringValue,
                sp_Tooltip.stringValue,
                sp_Color.colorValue,
                sp_Asset.objectReferenceValue);

            DrawEditButton(editButtonRect, index);
            DrawDeleteButton(deleteButtonRect, index);
        }

        private void DrawPingButton(Rect rect, string name, string tooltip, Color color, UnityEngine.Object asset)
        {
            var buttonContent = new GUIContent()
            {
                text = name,
                tooltip = tooltip
            };

            using (new SnekGUIColoredScope(color))
            {
                bool buttonClicked = GUI.Button(rect, buttonContent, _pingButtonStyle);

                if(buttonClicked)
                    EditorGUIUtility.PingObject(asset);
            }
        }

        private void DrawEditButton(Rect rect, int index)
        {
            var buttonContent = new GUIContent()
            {
                image = _editButtonTexture,
                tooltip = "Modify bookmark data."
            };


            using (new SnekGUIColoredScope(Color.softYellow))
            {
                bool buttonClicked = GUI.Button(rect, buttonContent, _buttonStyle);

                if (buttonClicked)
                    _onEditButtonClick.Invoke(index); 
            }
        }

        private void DrawDeleteButton(Rect rect, int index)
        {
            using (new SnekGUIColoredScope(Color.softRed))
            {
                var buttonContent = new GUIContent()
                {
                    image = _deleteButtonTexture,
                    tooltip = "Delete bookmark."
                };

                bool buttonClicked = GUI.Button(rect, buttonContent, _buttonStyle);

                if (buttonClicked)
                    _onDeleteButtonClick.Invoke(index);
            }
        }

        public override void Draw()
        {
            if (!IsDataValid())
                return;

            InitializeStyles();

            base.Draw();
        }

        private void InitializeStyles()
        {
            if (_textHeaderStyle == null)
                _textHeaderStyle = SnekGUIStyles.BoldLabel(16, stretchWidth: true, stretchHeight: true);

            if (_pingButtonStyle == null)
                _pingButtonStyle = SnekGUIStyles.BoldTextButton(20);
        }

        private bool IsDataValid()
        {
            return _editButtonTexture != null
                && _deleteButtonTexture != null
                && _onEditButtonClick != null
                && _onDeleteButtonClick != null;
        }
    }
}
