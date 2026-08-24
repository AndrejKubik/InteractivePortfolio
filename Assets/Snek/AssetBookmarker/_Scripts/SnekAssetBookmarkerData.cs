using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.AssetBookmarker
{
    [SnekAutoGenerateAsset("Assets/Snek/AssetBookmarker/Data", nameof(SnekAssetBookmarkerData))]
    [UseSnekInspector]
    public class SnekAssetBookmarkerData : SnekScriptableObject
    {
        public Texture2D WindowIconTexture;
        public Texture2D DeleteButtonTexture;
        public Texture2D EditButtonTexture;

        [Space(10f)]
        public List<SnekAssetBookmark> Bookmarks = new();
    }
}
