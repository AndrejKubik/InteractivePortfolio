using Snek.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[UseSnekInspector]
[RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter))]
public class InfiniteCarousel : SnekMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;
    private ContentSizeFitter _contentSizeFitter;

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
    }

    protected override void OnInitializationSuccess()
    {
        _gridLayoutGroup.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
