using System;
using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.AssetBookmarker
{
    public class SnekAssetBookmarkEditorWindow : SnekWindow
    {
        private const float WindowMinWidth = 400f;
        private const float WindowMinHeight = 350f;

        private const float WindowPadding = 20f;
        private const float CreateBookmarkButtonHeight = 50f;

        private const int FieldLabelFontSize = 16;
        private const float FieldHeight = 25f;

        private SnekAssetBookmarkEditor _bookmarkEditor;

        private SerializedObject so_bookmarkCreator;
        private SerializedProperty sp_NewBookmark;

        private SerializedProperty sp_Name;
        private SerializedProperty sp_Color;
        private SerializedProperty sp_Tooltip;
        private SerializedProperty sp_Asset;

        private SnekInputField field_Name;
        private SnekColorField field_Color;
        private SnekObjectField<UnityEngine.Object> field_Asset;
        private SnekTextAreaField field_Tooltip;

        private GUIStyle _labelStyle;
        private GUIStyle _inputFieldStyle;

        private Action<SnekAssetBookmark> _onConfirmChangesButtonClick;
        private SnekAssetBookmark? _existingBookmark = null;

        public static void ShowWindow(Action<SnekAssetBookmark> onConfirmChangesButtonClick, SnekAssetBookmark? existingBookmark = null)
        {
            var window = GetWindow<SnekAssetBookmarkEditorWindow>(true);

            window._onConfirmChangesButtonClick = onConfirmChangesButtonClick;
            window._existingBookmark = existingBookmark;
            window.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
        }

        protected override void Initialize()
        {
            string titleText = _existingBookmark == null ?
                "Create new bookmark" : "Edit bookmark";

            titleContent = new GUIContent(titleText);

            _bookmarkEditor = CreateInstance<SnekAssetBookmarkEditor>();
            _bookmarkEditor.name = "AssetBookmarkEditor";
        }

        protected override void Validate()
        {
            if (!_bookmarkEditor)
                FailValidation("Bookmark creator object is missing.");

            if (_onConfirmChangesButtonClick == null)
                FailValidation("Create bookmark button callback is missing.");

            _labelStyle ??= SnekGUIStyles.Label(FieldLabelFontSize, stretchHeight: true);
            _inputFieldStyle ??= SnekGUIStyles.TextField(FieldLabelFontSize);
        }

        protected override void OnInitializationSuccess()
        {
            if (_existingBookmark != null)
                _bookmarkEditor.NewBookmark = _existingBookmark.Value;

            so_bookmarkCreator = new SerializedObject(_bookmarkEditor);
            sp_NewBookmark = so_bookmarkCreator.FindProperty(nameof(SnekAssetBookmarkEditor.NewBookmark));

            sp_Name = sp_NewBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Name));
            sp_Color = sp_NewBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Color));
            sp_Tooltip = sp_NewBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Tooltip));
            sp_Asset = sp_NewBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Asset));

            field_Name = new SnekInputField(sp_Name, "Name", false, 0f, FieldHeight);
            field_Color = new SnekColorField(sp_Color, "Color", 0f, FieldHeight);

            field_Asset = new SnekObjectField<UnityEngine.Object>(
                sp_Asset,
                "Asset",
                false,
                GUILayout.Height(FieldHeight));

            field_Tooltip = new SnekTextAreaField(sp_Tooltip, "Tooltip", 0f, FieldHeight);
        }

        protected override bool IsReinitializationRequired()
        {
            return so_bookmarkCreator == null
                || sp_NewBookmark == null
                || sp_Color == null
                || sp_Tooltip == null
                || sp_Asset == null;
        }

        protected override void OnBeforeReinitialize()
        {
            OnDestroy();
        }

        private void OnDestroy()
        {
            if (_bookmarkEditor)
                DestroyImmediate(_bookmarkEditor);
        }



        private bool IsNewBookmarkNameValid()
        {
            return !string.IsNullOrWhiteSpace(sp_Name.stringValue);
        }

        private bool IsNewBookmarkAssetReferenceValid()
        {
            return sp_Asset.objectReferenceValue != null;
        }

        private bool IsNewBookmarkColorValid()
        {
            return sp_Color.colorValue != default && sp_Color.colorValue.a > 0f;
        }

        private bool IsNewBookmarkDataValid()
        {
            return IsNewBookmarkNameValid()
                && IsNewBookmarkColorValid()
                && IsNewBookmarkAssetReferenceValid();
        }

        private bool IsAutoNameAllowed()
        {
            return !IsNewBookmarkNameValid() || sp_Name.stringValue == SnekAssetBookmarkEditor.DefaultName;
        }



        protected override void DrawContent()
        {
            GUILayout.Space(WindowPadding);

            using (new SnekGUIHorizontalScope())
            {
                GUILayout.Space(WindowPadding);

                using (new SnekGUIVerticalScope())
                {
                    DrawBookmarkCreatorFields();

                    GUILayout.FlexibleSpace();

                    using (new EditorGUI.DisabledScope(!IsNewBookmarkDataValid()))
                        DrawConfirmChangesButton();
                }

                GUILayout.Space(WindowPadding);
            }

            GUILayout.Space(WindowPadding);
        }

        private void DrawBookmarkCreatorFields()
        {
            so_bookmarkCreator.Update();

            DrawNameField();

            GUILayout.Space(5f);

            DrawColorField();

            GUILayout.Space(5f);

            DrawAssetReferenceField();

            GUILayout.Space(15f);

            field_Tooltip.DrawHorizontal(_labelStyle);

            so_bookmarkCreator.ApplyModifiedProperties();
        }

        private void DrawNameField()
        {
            field_Name.DrawHorizontal(_labelStyle, _inputFieldStyle);

            if (!IsNewBookmarkNameValid())
                EditorGUILayout.HelpBox("Name cannot be empty.", MessageType.Error);
        }

        private void DrawColorField()
        {
            if (!IsNewBookmarkColorValid())
                EditorGUILayout.HelpBox("Color must have alpha bigger than 0.", MessageType.Error);

            field_Color.DrawHorizontal(_labelStyle);
        }

        private void DrawAssetReferenceField()
        {
            using (var scope = new SnekPropertyFieldScope(sp_Asset))
            {
                field_Asset.DrawHorizontal(_labelStyle);

                if (scope.IsValueChanged() && IsAutoNameAllowed())
                {
                    sp_Name.stringValue = sp_Asset.objectReferenceValue.name;

                    so_bookmarkCreator.ApplyModifiedProperties();
                }
            }

            if (!IsNewBookmarkAssetReferenceValid())
                EditorGUILayout.HelpBox("No asset assigned.", MessageType.Error);
        }

        private void DrawConfirmChangesButton()
        {
            string text = _existingBookmark == null ?
                "Create bookmark" : "Save changes";

            bool buttonClicked = GUILayout.Button(
                text,
                GUILayout.Height(CreateBookmarkButtonHeight));

            if (buttonClicked)
            {
                if (sp_Tooltip.stringValue == SnekAssetBookmarkEditor.DefaultTooltip)
                {
                    sp_Tooltip.stringValue = string.Empty;

                    so_bookmarkCreator.ApplyModifiedProperties();
                }

                _onConfirmChangesButtonClick.Invoke(_bookmarkEditor.NewBookmark);

                Close();
            }
        }
    }
}
