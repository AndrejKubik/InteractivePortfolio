using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[UseSnekInspector]
[RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter))]
public class EndlessCarousel : SnekMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;
    private ContentSizeFitter _contentSizeFitter;

    public float ElementWidth = 200f;
    public float ElementHeight = 200f;
    public int ElementSpacing = 10;

    private bool _isDragging = false;
    private Vector2 _currentPointerPosition;
    private Vector2 _lastPointerPosition;
    private float _velocity = 0f;

    private List<IEndlessCarouselElement> _elements;

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

        FindAllElements();
    }

    private void Update()
    {
        if (IsEmpty()/* || !IsMovingAllowed()*/)
            return;

        if (_isDragging)
        {
            Vector2 dragDelta = _currentPointerPosition - _lastPointerPosition;

            ApplyDragVelocity(dragDelta);
            MoveElementsHorizontally(dragDelta.x);

            _lastPointerPosition = _currentPointerPosition;
        }
        else
        {
            ApplyInertiaVelocity();
            MoveElementsHorizontally(_velocity * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;

        _lastPointerPosition = eventData.position;
        _currentPointerPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _currentPointerPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }

    private void FindAllElements()
    {
        IEndlessCarouselElement[] elements = GetComponentsInChildren<IEndlessCarouselElement>(true);

        _elements = new List<IEndlessCarouselElement>(elements);
    }

    private bool IsEmpty()
    {
        return _elements == null || _elements.Count < 1;
    }

    private bool IsMovingAllowed()
    {
        float perElementRequiredSpace = ElementWidth + ElementSpacing;
        float totalPadding = _gridLayoutGroup.padding.left + _gridLayoutGroup.padding.right;

        float totalRequiredSpace = _elements.Count * perElementRequiredSpace + totalPadding;
        totalRequiredSpace -= ElementSpacing; //spacing is only in-between elements, so its not needed after the last element

        return _rectTransform.rect.width - totalRequiredSpace < 0f;
    }

    private void ApplyDragVelocity(Vector2 dragDelta)
    {
        _velocity = dragDelta.x / Time.deltaTime;
    }

    private void ApplyInertiaVelocity()
    {
        _velocity = Mathf.Lerp(_velocity, 0f, Time.deltaTime * 7.5f);
    }

    private void MoveElementsHorizontally(float positionOffset)
    {
        if (Mathf.Approximately(positionOffset, 0f))
            return;

        float totalWidth = _rectTransform.rect.width;
        float loopBoundOffset = ElementWidth / 2f + ElementSpacing;

        if (positionOffset > 0f)
            MoveElementsRight(positionOffset, totalWidth + loopBoundOffset);
        else if (positionOffset < 0f)
            MoveElementsLeft(positionOffset, -loopBoundOffset);
    }

    private void MoveElementsRight(float positionOffset, float loopTriggerPositionX)
    {
        IEndlessCarouselElement loopInitiatorElement = null;

        for (int i = _elements.Count - 1; i >= 0; i--)
        {
            IEndlessCarouselElement element = _elements[i];
            RectTransform rectTransform = element.GetRectTransform();

            Vector2 currentPosition = rectTransform.anchoredPosition;
            Vector2 targetPosition = currentPosition;

            targetPosition.x += positionOffset;

            if (loopInitiatorElement != null)
            {
                rectTransform.anchoredPosition = targetPosition;

                continue;
            }

            if (targetPosition.x > loopTriggerPositionX)
                loopInitiatorElement = element;

            rectTransform.anchoredPosition = targetPosition;
        }

        if (loopInitiatorElement != null)
            LoopElementRight(loopInitiatorElement);
    }

    private void MoveElementsLeft(float positionOffset, float loopTriggerPositionX)
    {
        IEndlessCarouselElement loopInitiatorElement = null;

        for (int i = 0; i < _elements.Count; i++)
        {
            IEndlessCarouselElement element = _elements[i];
            RectTransform rectTransform = element.GetRectTransform();

            Vector2 currentPosition = rectTransform.anchoredPosition;
            Vector2 targetPosition = currentPosition;

            targetPosition.x += positionOffset;

            if (loopInitiatorElement != null)
            {
                rectTransform.anchoredPosition = targetPosition;

                continue;
            }

            if (targetPosition.x < loopTriggerPositionX)
                loopInitiatorElement = element;

            rectTransform.anchoredPosition = targetPosition;
        }

        if (loopInitiatorElement != null)
            LoopElementLeft(loopInitiatorElement);
    }

    private void LoopElementLeft(IEndlessCarouselElement element)
    {
        RectTransform rectTransform = element.GetRectTransform();
        RectTransform rightMostTransform = _elements[^1].GetRectTransform();

        Vector2 targetPosition = rightMostTransform.anchoredPosition;
        targetPosition.x += ElementWidth + ElementSpacing;

        rectTransform.anchoredPosition = targetPosition;

        _elements.Remove(element);
        _elements.Add(element);
    }

    private void LoopElementRight(IEndlessCarouselElement element)
    {
        RectTransform rectTransform = element.GetRectTransform();
        RectTransform leftMostTransform = _elements[0].GetRectTransform();

        Vector2 targetPosition = leftMostTransform.anchoredPosition;
        targetPosition.x -= ElementWidth + ElementSpacing;

        rectTransform.anchoredPosition = targetPosition;

        _elements.Remove(element);
        _elements.Insert(0, element);
    }
}
