using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(EndlessCarousel))]
public class EndlessCarouselInspector : SnekMonoBehaviourInspectorCustom<EndlessCarousel>
{
    private EndlessCarousel _endlessCarousel;

    private GridLayoutGroup _gridLayoutGroup;
    private ContentSizeFitter _contentSizeFitter;

    private SerializedProperty sp_ElementWidth;
    private SerializedProperty sp_ElementHeight;
    private SerializedProperty sp_ElementSpacing;

    private SnekInputField _elementWidthField;
    private SnekInputField _elementHeightField;
    private SnekInputField _elementSpacingField;

    protected override void OnCreateInspectorInstance()
    {
        _endlessCarousel = GetSelectedComponent();

        if (_endlessCarousel.TryGetComponent(out _gridLayoutGroup))
            _gridLayoutGroup.hideFlags = HideFlags.NotEditable;

        if (_endlessCarousel.TryGetComponent(out _contentSizeFitter))
            _contentSizeFitter.hideFlags = HideFlags.NotEditable;

        sp_ElementWidth = GetProperty(nameof(_endlessCarousel.ElementWidth));
        sp_ElementHeight = GetProperty(nameof(_endlessCarousel.ElementHeight));
        sp_ElementSpacing = GetProperty(nameof(_endlessCarousel.ElementSpacing));

        _elementWidthField = new SnekInputField(sp_ElementWidth, "Width", 0f);
        _elementHeightField = new SnekInputField(sp_ElementHeight, "Height", 0f);
        _elementSpacingField = new SnekInputField(sp_ElementSpacing, "Spacing", 0f);
    }

    protected override bool Initialize()
    {
        return base.Initialize()
            && _gridLayoutGroup != null
            && _contentSizeFitter != null;
    }

    protected override void DrawContent()
    {
        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
            GUILayout.Label("Element Settings", _sectionHeaderStyle);

        using (new SnekGUIHorizontalScope())
        {
            _elementWidthField.DrawVertical();
            _elementHeightField.DrawVertical();
            _elementSpacingField.DrawVertical();
        }
    }

    protected override void OnPropertiesChange()
    {
        _gridLayoutGroup.cellSize = new Vector2(
            _endlessCarousel.ElementWidth,
            _endlessCarousel.ElementHeight);

        int elementSpacing = Mathf.RoundToInt(_endlessCarousel.ElementSpacing);

        _gridLayoutGroup.spacing = new Vector2(elementSpacing, 0);
        _gridLayoutGroup.padding = SnekGUIUtility.Get2DPadding(elementSpacing, 0);

        EditorUtility.SetDirty(_gridLayoutGroup);
    }

    private void OnDisable()
    {
        if (_gridLayoutGroup)
            _gridLayoutGroup.hideFlags = HideFlags.None;

        if (_contentSizeFitter)
            _contentSizeFitter.hideFlags = HideFlags.None;
    }
}
