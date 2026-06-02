using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
[RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter))]
public class EndlessCarouselElementContainer : SnekMonoBehaviour
{
    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;
    private ContentSizeFitter _contentSizeFitter;

    protected override bool IsInitializedInStart()
    {
        return true;
    }

    protected override void Initialize()
    {
        GetEssentialComponent(out _rectTransform);
        GetEssentialComponent(out _gridLayoutGroup);
        GetEssentialComponent(out _contentSizeFitter);
    }

    protected override void Validate()
    {
        if (_gridLayoutGroup.enabled == false)
            FailValidation("Grid Layout Group is disabled, it must be enabled by default for correct item distribution.");

        if (_contentSizeFitter.enabled == false)
            FailValidation("Content size fitter is disabled, it must be enabled by default for correct item distribution.");
    }

    protected override void OnInitializationSuccess()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform); //enforces element position distribution before disabling the layout group

        _gridLayoutGroup.enabled = false;
        _contentSizeFitter.enabled = false;
    }

    public float GetTotalWidth()
    {
        return _rectTransform.rect.width;
    }
}
