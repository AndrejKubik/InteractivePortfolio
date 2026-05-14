using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    [SnekAutoGenerateAsset("Assets/Snek/EditorGUIUtilities/CustomWindow", nameof(SnekWindowGUILayoutSettings))]
    [UseSnekInspector]
    public class SnekWindowGUILayoutSettings : SnekScriptableObject
    {
        [SerializeField] private Color _borderColor;
        [SerializeField] private float _borderWidth = 5f;

        [Space(10f)]
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _contentColor;

        public SnekWindowGUILayoutSettings()
        {
            ColorUtility.TryParseHtmlString("#00C7FF", out _borderColor);
            ColorUtility.TryParseHtmlString("#000000", out _backgroundColor);
            ColorUtility.TryParseHtmlString("#C0F2FF", out _contentColor);
        }

        public float GetBorderWidth()
        {
            return _borderWidth;
        }

        public Color GetBorderColor()
        {
            return _borderColor;
        }

        public Color GetBackgroundColor()
        {
            return _backgroundColor;
        }

        public Color GetContentColor()
        {
            return _contentColor;
        }
    }
}
