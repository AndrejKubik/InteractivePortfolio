using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    [SnekAutoGenerateAsset("Assets/Snek/EditorGUIUtilities/CustomInspector", nameof(SnekInspectorGUILayoutSettings))]
    [UseSnekInspector]
    public class SnekInspectorGUILayoutSettings : SnekScriptableObject
    {
        [SerializeField] private float _borderWidth = 2f;

        [SerializeField] private RectOffset _componentPadding;
        [SerializeField] private RectOffset _sectionPadding;

        [Space(10f)]
        [SerializeField] private Color _borderColor;
        [SerializeField] private Color _disabledBorderColor;

        [Space(10f)]
        [SerializeField] private Color _backgroundColor;

        [Space(10f)]
        [SerializeField] private Color _contentColor;
        [SerializeField] private Color _disabledContentColor;

        [Space(10f)]
        [SerializeField] private Vector2 _backgroundSizeOffset = new Vector2(21f, 12f);
        [SerializeField] private Vector2 _backgroundPositionOffset = new Vector2(-18f, -5f);
        
        [Space(10f)]
        [SerializeField] private List<Color> _listElementColors;

        public SnekInspectorGUILayoutSettings()
        {
            InitializeDefaultBorderColors();

            ColorUtility.TryParseHtmlString("#000000", out _backgroundColor);

            InitializeDefaultContentColors();
            InitializeDefaultListElementColors();
        }

        private void OnEnable()
        {
            InitializeDefaultPaddingValues();
        }

        private void InitializeDefaultPaddingValues()
        {
            if (_componentPadding == null)
                _componentPadding = new RectOffset(0, 13, 10, 10);

            if (_sectionPadding == null)
                _sectionPadding = new RectOffset(25, 25, 15, 15);
        }

        private void InitializeDefaultContentColors()
        {
            ColorUtility.TryParseHtmlString("#C0F2FF", out _contentColor);
            ColorUtility.TryParseHtmlString("#A6A6A6", out _disabledContentColor);
        }

        private void InitializeDefaultBorderColors()
        {
            ColorUtility.TryParseHtmlString("#00C7FF", out _borderColor);
            ColorUtility.TryParseHtmlString("#A6A6A6", out _disabledBorderColor);
        }

        private void InitializeDefaultListElementColors()
        {
            ColorUtility.TryParseHtmlString("#454545", out Color color1);
            ColorUtility.TryParseHtmlString("#333333", out Color color2);

            _listElementColors = new List<Color>() { color1, color2 };
        }

        public RectOffset GetComponentPadding()
        {
            return _componentPadding;
        }

        public RectOffset GetSectionPadding()
        {
            return _sectionPadding;
        }

        public float GetBorderWidth()
        {
            return _borderWidth;
        }

        public Color GetBorderColor()
        {
            return _borderColor;
        }

        public Color GetDisabledBorderColor()
        {
            return _disabledBorderColor;
        }

        public Color GetBackgroundColor()
        {
            return _backgroundColor;
        }

        public Color GetContentColor()
        {
            return _contentColor;
        }

        public Color GetDisabledContentColor()
        {
            return _disabledContentColor;
        }

        public Vector2 GetBackgroundSizeOffset()
        {
            return _backgroundSizeOffset;
        }

        public Vector2 GetBackgroundPositionOffset()
        {
            return _backgroundPositionOffset;
        }

        public List<Color> GetListElementColors()
        {
            return _listElementColors;
        }
    }
}
