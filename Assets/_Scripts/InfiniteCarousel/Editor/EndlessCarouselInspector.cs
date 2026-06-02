using System;
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

    private Action _onPropertiesChanged;

    protected override void OnCreateInspectorInstance()
    {
        InitializeDependencies();

        if (!IsEveryDependencyInitialized())
            return;

        SyncGridLayoutGroupValues();
        EnforceContentSizeFitterValues();

        sp_ElementWidth = GetProperty(nameof(_endlessCarousel.ElementWidth));
        sp_ElementHeight = GetProperty(nameof(_endlessCarousel.ElementHeight));
        sp_ElementSpacing = GetProperty(nameof(_endlessCarousel.ElementSpacing));

        _elementWidthField = new SnekInputField(sp_ElementWidth, "Width", 0f);
        _elementHeightField = new SnekInputField(sp_ElementHeight, "Height", 0f);
        _elementSpacingField = new SnekInputField(sp_ElementSpacing, "Spacing", 0f);
    }

    protected override bool Initialize()
    {
        return base.Initialize() && IsEveryDependencyInitialized();
    }

    protected override void DrawContent()
    {
        using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
            GUILayout.Label("Element Settings", _sectionHeaderStyle);

        using (new SnekGUIHorizontalScope())
        {
            DrawElementWidthField();
            DrawElementHeightField();
            DrawElementSpacingField();
        }
    }

    private void OnDisable()
    {
        if (!IsEveryDependencyInitialized())
            return;

        //_gridLayoutGroup.hideFlags = HideFlags.None;
        //_contentSizeFitter.hideFlags = HideFlags.None;
    }

    protected override void OnPropertiesChange()
    {
        _onPropertiesChanged?.Invoke();
        _onPropertiesChanged = null;
    }

    private void DrawElementWidthField()
    {
        using (var propertyScope = new SnekPropertyFieldScope(sp_ElementWidth))
        {
            _elementWidthField.DrawVertical();

            if (propertyScope.IsValueChanged())
                _onPropertiesChanged += SyncCellSize;
        }
    }

    private void DrawElementHeightField()
    {
        using (var propertyScope = new SnekPropertyFieldScope(sp_ElementHeight))
        {
            _elementHeightField.DrawVertical();

            if (propertyScope.IsValueChanged())
                _onPropertiesChanged += SyncCellSize;
        }
    }

    private void DrawElementSpacingField()
    {
        using (var propertyScope = new SnekPropertyFieldScope(sp_ElementSpacing))
        {
            _elementSpacingField.DrawVertical();

            if (propertyScope.IsValueChanged())
                _onPropertiesChanged += SyncElementSpacingAndPadding;
        }
    }

    private void InitializeDependencies()
    {
        _endlessCarousel = GetSelectedComponent();

        _endlessCarousel.TryGetComponent(out _gridLayoutGroup);
        _endlessCarousel.TryGetComponent(out _contentSizeFitter);

        //if (_endlessCarousel.TryGetComponent(out _gridLayoutGroup))
        //    _gridLayoutGroup.hideFlags = HideFlags.NotEditable;

        //if (_endlessCarousel.TryGetComponent(out _contentSizeFitter))
        //    _contentSizeFitter.hideFlags = HideFlags.NotEditable;
    }

    private bool IsEveryDependencyInitialized()
    {
        return _gridLayoutGroup != null 
            && _contentSizeFitter != null
            && _endlessCarousel != null;
    }

    private void SyncGridLayoutGroupValues()
    {
        Vector2 correctElementSize = new Vector2(
            _endlessCarousel.ElementWidth, 
            _endlessCarousel.ElementHeight);

        if (_gridLayoutGroup.cellSize != correctElementSize)
            SyncCellSize();

        float spacing = _gridLayoutGroup.spacing.x;
        RectOffset padding = _gridLayoutGroup.padding;
        RectOffset correctPadding = SnekGUIUtility.Get2DPadding(_endlessCarousel.ElementSpacing, 0);

        if (spacing != _endlessCarousel.ElementSpacing || !SnekGUIUtility.IsRectOffsetEqual(padding, correctPadding))
            SyncElementSpacingAndPadding();
    }

    private void SyncElementSpacingAndPadding()
    {
        _gridLayoutGroup.spacing = new Vector2(_endlessCarousel.ElementSpacing, 0);
        _gridLayoutGroup.padding = SnekGUIUtility.Get2DPadding(_endlessCarousel.ElementSpacing, 0);

        EditorUtility.SetDirty(_gridLayoutGroup);
    }

    private void SyncCellSize()
    {
        _gridLayoutGroup.cellSize = new Vector2(
            _endlessCarousel.ElementWidth,
            _endlessCarousel.ElementHeight);

        EditorUtility.SetDirty(_gridLayoutGroup);
    }

    private void EnforceContentSizeFitterValues()
    {
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}
