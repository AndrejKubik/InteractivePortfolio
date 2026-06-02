using System;
using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(EndlessCarousel))]
public class EndlessCarouselInspector : SnekMonoBehaviourInspectorCustom<EndlessCarousel>
{
    private EndlessCarousel _endlessCarousel;
    private EndlessCarouselElementContainer _elementContainer;

    private GridLayoutGroup _gridLayoutGroup;
    private ContentSizeFitter _contentSizeFitter;

    private SerializedProperty sp_ElementContainer;
    private SerializedProperty sp_ElementWidth;
    private SerializedProperty sp_ElementHeight;
    private SerializedProperty sp_ElementSpacing;

    private SnekObjectField<EndlessCarouselElementContainer> _elementContainerField;
    private SnekInputField _elementWidthField;
    private SnekInputField _elementHeightField;
    private SnekInputField _elementSpacingField;

    private Action _onPropertiesChanged;

    protected override void OnCreateInspectorInstance()
    {
        _endlessCarousel = GetSelectedComponent();

        sp_ElementContainer = GetProperty(nameof(_endlessCarousel.ElementContainer));
        sp_ElementWidth = GetProperty(nameof(_endlessCarousel.ElementWidth));
        sp_ElementHeight = GetProperty(nameof(_endlessCarousel.ElementHeight));
        sp_ElementSpacing = GetProperty(nameof(_endlessCarousel.ElementSpacing));

        _elementContainerField = new SnekObjectField<EndlessCarouselElementContainer>(sp_ElementContainer, "Element Container", true);
        _elementWidthField = new SnekInputField(sp_ElementWidth, "Width", 0f);
        _elementHeightField = new SnekInputField(sp_ElementHeight, "Height", 0f);
        _elementSpacingField = new SnekInputField(sp_ElementSpacing, "Spacing", 0f);

        InitializeDependencies();

        if (!_elementContainer)
            return;

        if (!IsContentSizeFitterDataSynced())
            EnforceContentSizeFitterValues();

        if (!IsBaseGridLayoutGroupDataSynced())
            EnforceBaseGridLayoutGroupValues();

        SyncGridLayoutGroupValues();
    }

    protected override void DrawContent()
    {
        DrawElementContainerField();

        GUILayout.Space(10f);

        if (!_elementContainer)
        {
            SnekGUILayout.DrawAlertMessage("No element container assigned.");

            return;
        }

        using (new SnekGUISectionScope("Layout Settings", _sectionHeaderStyle, _sectionStyle))
        {
            using (new SnekGUIHorizontalScope())
            {
                DrawElementWidthField();
                DrawElementHeightField();
                DrawElementSpacingField();
            }
        }
    }

    protected override void OnPropertiesChange()
    {
        _onPropertiesChanged?.Invoke();
        _onPropertiesChanged = null;
    }

    private void DrawElementContainerField()
    {
        using (var propertyScope = new SnekPropertyFieldScope(sp_ElementContainer))
        {
            _elementContainerField.DrawHorizontal();

            if (propertyScope.IsValueChanged())
                _onPropertiesChanged += InitializeDependencies;
        }
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
        _elementContainer = _endlessCarousel.ElementContainer;

        if (!_elementContainer)
            return;

        _elementContainer.TryGetComponent(out _gridLayoutGroup);
        _elementContainer.TryGetComponent(out _contentSizeFitter);
    }

    private void SyncGridLayoutGroupValues()
    {
        if (!IsElementSizeSynced())
            SyncCellSize();

        if (!IsElementSpacingSynced() || !IsElementPaddingSynced())
            SyncElementSpacingAndPadding();
    }

    private bool IsElementSizeSynced()
    {
        Vector2 correctElementSize = new Vector2(
            _endlessCarousel.ElementWidth,
            _endlessCarousel.ElementHeight);

        return _gridLayoutGroup.cellSize == correctElementSize;
    }

    private bool IsElementSpacingSynced()
    {
        return _gridLayoutGroup.spacing.x == _endlessCarousel.ElementSpacing;
    }

    private bool IsElementPaddingSynced()
    {
        RectOffset correctPadding = SnekGUIUtility.Get2DPadding(_endlessCarousel.ElementSpacing, 0);

        return SnekGUIUtility.IsRectOffsetEqual(_gridLayoutGroup.padding, correctPadding);
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

    private void EnforceBaseGridLayoutGroupValues()
    {
        if (IsBaseGridLayoutGroupDataSynced())
            return;

        _gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        _gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        _gridLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _gridLayoutGroup.constraintCount = 1;

        EditorUtility.SetDirty(_gridLayoutGroup);
    }

    private bool IsBaseGridLayoutGroupDataSynced()
    {
        return _gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperLeft
            && _gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal
            && _gridLayoutGroup.childAlignment == TextAnchor.MiddleLeft
            && _gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount
            && _gridLayoutGroup.constraintCount == 1;
    }

    private void EnforceContentSizeFitterValues()
    {
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        EditorUtility.SetDirty(_contentSizeFitter);
    }

    private bool IsContentSizeFitterDataSynced()
    {
        return _contentSizeFitter.horizontalFit == ContentSizeFitter.FitMode.PreferredSize
            && _contentSizeFitter.verticalFit == ContentSizeFitter.FitMode.PreferredSize;
    }
}
