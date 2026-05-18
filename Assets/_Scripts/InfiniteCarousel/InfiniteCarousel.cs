using System.Collections.Generic;
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

    private bool _isDragging = false;
    private Vector2 _currentPointerPosition;
    private Vector2 _lastPointerPosition;
    private float _velocity = 0f;

    private List<IInfiniteCarouselElement> _elements;

    private Vector2 _elementSize = new Vector2(0f, 0f);
    private Vector2 _elementSpacing = new Vector2(0f, 0f);

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

        if (IsEmpty())
            return;

        _elementSize = _gridLayoutGroup.cellSize;
        _elementSpacing = _gridLayoutGroup.spacing;
    }

    private void Update()
    {
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
        IInfiniteCarouselElement[] elements = GetComponentsInChildren<IInfiniteCarouselElement>(true);

        _elements = new List<IInfiniteCarouselElement>(elements);
    }

    private bool IsEmpty()
    {
        return _elements == null || _elements.Count < 1;
    }

    private bool IsStationary()
    {
        return Mathf.Approximately(_velocity, 0f);
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
        //float totalWidth = _rectTransform.sizeDelta.x;

        //bool loopTriggeredLeft = false;
        //bool loopTriggeredRight = false;
        //IInfiniteCarouselElement loopInitiatorElement = null;

        foreach (IInfiniteCarouselElement element in _elements)
        {
            RectTransform rectTransform = element.GetRectTransform();

            Vector2 targetPosition = rectTransform.anchoredPosition;
            targetPosition.x += positionOffset;

            //if (targetPosition.x < 0f)
            //{
            //    RectTransform lastElementTransform = _elements[^1].GetRectTransform();

            //    targetPosition = lastElementTransform.anchoredPosition;
            //    targetPosition.x += _elementSize.x + _elementSpacing.x;

            //    loopTriggeredLeft = true;
            //    loopInitiatorElement = element;
            //}
            //else if (targetPosition.x > totalWidth)
            //{
            //    RectTransform firstElementTransform = _elements[0].GetRectTransform();

            //    targetPosition = firstElementTransform.anchoredPosition;
            //    targetPosition.x -= _elementSize.x + _elementSpacing.x;

            //    loopTriggeredRight = true;
            //    loopInitiatorElement = element;
            //}

            rectTransform.anchoredPosition = targetPosition;
        }

        //if (loopInitiatorElement == null)
        //    return;

        //_elements.Remove(loopInitiatorElement);

        //if (loopTriggeredLeft)
        //    _elements.Add(loopInitiatorElement);
        //else if (loopTriggeredRight)
        //    _elements.Insert(0, loopInitiatorElement);
    }
}
