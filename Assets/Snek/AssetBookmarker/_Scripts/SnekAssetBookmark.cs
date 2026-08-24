using UnityEngine;

namespace SnekEditor.AssetBookmarker
{
    [System.Serializable]
    public struct SnekAssetBookmark
    {
        public string Name;
        public Color Color;

        [TextArea(1, 10)]
        public string Tooltip;

        public Object Asset;
    }
}
