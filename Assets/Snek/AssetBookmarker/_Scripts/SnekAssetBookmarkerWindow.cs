using SnekEditor.GUIUtilities;
using SnekEditor.ScriptableObjectManager;
using SnekEditor.Utilities;
using Snek.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.AssetBookmarker
{
    public class SnekAssetBookmarkerWindow : SnekWindow
    {
        private const float WindowContentPadding = 10f;
        private const float NewBookmarkButtonHeight = 30f;

        private const int NoSelectionIndex = -1;

        private GUIStyle _buttonStyle;

        private SnekAssetBookmarkerData _data;
        private SerializedObject so_data;

        private SerializedProperty sp_Bookmarks;

        private Vector2 _scrollPosition = Vector2.zero;
        private int _editedBookmarkIndex = NoSelectionIndex;

        private SnekAssetBookmarkerList list_Bookmarks;

        private int _deletedBookmarkIndex = NoSelectionIndex;


        [MenuItem(SnekEditorUtility.MenuItemRoot + "Asset Bookmarker")]
        private static void ShowWindow()
        {
            GetWindow<SnekAssetBookmarkerWindow>();
        }

        protected override void Initialize()
        {
            _data = SnekScriptableObjectManager.GetAsset<SnekAssetBookmarkerData>();
        }

        protected override void Validate()
        {
            if (_buttonStyle == null)
                _buttonStyle = SnekGUIStyles.BoldTextButton(20);

            if (!_data)
                FailValidation("Cannot find asset bookmarker data.");
            else
            {
                if (!_data.DeleteButtonTexture)
                    FailValidation("Delete button texture not assigned.");

                if (!_data.EditButtonTexture)
                    FailValidation("Edit button texture not assigned.");

                if (!_data.WindowIconTexture)
                    FailValidation("Window icon texture not assigned.");
            }
        }

        protected override void OnInitializationSuccess()
        {
            titleContent = new GUIContent("Asset Bookmarker", _data.WindowIconTexture);

            so_data = new SerializedObject(_data);
            sp_Bookmarks = so_data.FindProperty(nameof(SnekAssetBookmarkerData.Bookmarks));

            list_Bookmarks = new SnekAssetBookmarkerList(
                so_data,
                sp_Bookmarks,
                _data.EditButtonTexture,
                _data.DeleteButtonTexture,
                StartBookmarkEdit,
                RequestDeleteBookmark);

            Undo.undoRedoEvent += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoEvent -= OnUndoRedo;
        }

        private void OnUndoRedo(in UndoRedoInfo undo)
        {
            Repaint();
        }

        protected override bool IsReinitializationRequired()
        {
            return so_data == null
                || sp_Bookmarks == null
                || list_Bookmarks == null;
        }



        protected override void DrawContent()
        {
            so_data.Update();

            if(IsDeleteBookmarkRequested()) //this approach prevents GUI errors when deleting a non-last element from the list
            {
                DeleteBookmark();
                Repaint();
            }

            using(var scrollViewScope = new GUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollViewScope.scrollPosition;

                using (new SnekGUIHorizontalScope())
                {
                    GUILayout.Space(WindowContentPadding);

                    using (new SnekGUIVerticalScope())
                    {
                        GUILayout.Space(WindowContentPadding);

                        DrawNewBookmarkButton();

                        GUILayout.Space(20f);

                        list_Bookmarks.Draw();

                        GUILayout.Space(WindowContentPadding);
                    }

                    GUILayout.Space(WindowContentPadding);
                }
            }
        }

        private void DrawNewBookmarkButton()
        {
            bool buttonClicked = GUILayout.Button(
                "New Bookmark",
                _buttonStyle,
                GUILayout.Height(NewBookmarkButtonHeight));

            if (buttonClicked)
                SnekAssetBookmarkEditorWindow.ShowWindow(CreateNewBookmark);
        }


        
        private void StartBookmarkEdit(int bookmarkIndex)
        {
            _editedBookmarkIndex = bookmarkIndex;

            SnekAssetBookmarkEditorWindow.ShowWindow(FinishBookmarkEdit, _data.Bookmarks[bookmarkIndex]);
        }

        private void FinishBookmarkEdit(SnekAssetBookmark editedBookmark)
        {
            ApplyDataToBookmark(_editedBookmarkIndex, editedBookmark);

            _editedBookmarkIndex = NoSelectionIndex;
        }

        private void ApplyDataToBookmark(int bookmarkIndex, SnekAssetBookmark bookmarkData)
        {
            if(!_data.Bookmarks.HasIndex(bookmarkIndex))
            {
                Debug.LogError("Requested index does not exist in asset bookmarks list, cannot edit bookmark.");

                return;
            }

            SerializedProperty sp_newBookmark = sp_Bookmarks.GetArrayElementAtIndex(bookmarkIndex);

            SerializedProperty sp_Name = sp_newBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Name));
            SerializedProperty sp_Color = sp_newBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Color));
            SerializedProperty sp_Tooltip = sp_newBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Tooltip));
            SerializedProperty sp_Asset = sp_newBookmark.FindPropertyRelative(nameof(SnekAssetBookmark.Asset));

            sp_Name.stringValue = bookmarkData.Name;
            sp_Color.colorValue = bookmarkData.Color;
            sp_Tooltip.stringValue = bookmarkData.Tooltip;
            sp_Asset.objectReferenceValue = bookmarkData.Asset;

            so_data.ApplyModifiedProperties();
            
            Repaint();
        }

        private void CreateNewBookmark(SnekAssetBookmark newBookmark)
        {
            sp_Bookmarks.InsertArrayElementAtIndex(0);

            so_data.ApplyModifiedProperties();

            ApplyDataToBookmark(0, newBookmark);
        }

        private void RequestDeleteBookmark(int bookmarkIndex)
        {
            if (!_data.Bookmarks.HasIndex(bookmarkIndex))
            {
                Debug.LogError("Requested index does not exist in asset bookmarks list, cannot delete bookmark.");

                return;
            }

            _deletedBookmarkIndex = bookmarkIndex;
        }

        private void DeleteBookmark()
        {
            if (!_data.Bookmarks.HasIndex(_deletedBookmarkIndex))
            {
                Debug.LogError("Requested index does not exist in asset bookmarks list, cannot delete bookmark.");

                return;
            }

            sp_Bookmarks.DeleteArrayElementAtIndex(_deletedBookmarkIndex);

            _deletedBookmarkIndex = NoSelectionIndex;

            so_data.ApplyModifiedProperties();
        }

        private bool IsDeleteBookmarkRequested()
        {
            return _deletedBookmarkIndex != NoSelectionIndex;
        }
    }
}
