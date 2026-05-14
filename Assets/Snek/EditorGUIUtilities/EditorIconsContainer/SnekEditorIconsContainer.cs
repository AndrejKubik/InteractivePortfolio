using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    [SnekAutoGenerateAsset("Assets/Snek/EditorGUIUtilities/EditorIconsContainer", nameof(SnekEditorIconsContainer))]
    [UseSnekInspector]
    public class SnekEditorIconsContainer : SnekScriptableObject
    {
        [SerializeField] private Texture2D _logoThin;
        [SerializeField] private Texture2D _logoThick;

        [SerializeField] private Texture2D _favouriteOff;
        [SerializeField] private Texture2D _favouriteOn;

        [SerializeField] private Texture2D _foldoutExpanded;
        [SerializeField] private Texture2D _foldoutShrinked;

        public Texture2D GetLogoThin()
        {
            return _logoThin;
        }

        public Texture2D GetLogoThick()
        {
            return _logoThick;
        }

        public Texture2D GetFavourite(bool state)
        {
            return state == true ? _favouriteOn : _favouriteOff;
        }

        public Texture2D GetFoldout(bool state)
        {
            return state == true ? _foldoutExpanded : _foldoutShrinked;
        }
    }
}
