using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.AssetBookmarker
{
    public class SnekAssetBookmarkEditor : SnekScriptableObject
    {
        public SnekAssetBookmark NewBookmark = new();

        public const string DefaultName = "New Asset Bookmark";
        public const string DefaultTooltip = "Extra information about the bookmark.";

        public SnekAssetBookmarkEditor()
        {
            NewBookmark.Name = DefaultName;
            NewBookmark.Tooltip = DefaultTooltip;
            NewBookmark.Color = Color.white;
        }
    }
}
