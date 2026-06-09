using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Snek.EndlessCarousel
{
    [UseSnekInspector]
    [RequireComponent(typeof(RectTransform))]
    public class SnekEndlessCarousel : SnekMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _rectTransform;

        public SnekEndlessCarouselElementContainer ElementContainer;
        public float ElementWidth = 200f;
        public float ElementHeight = 200f;
        public int ElementSpacing = 10;

        private List<ISnekEndlessCarouselElement> _elements;

        public bool IsDragging { get; private set; }

        private Vector2 _currentPointerPosition;
        private Vector2 _lastPointerPosition;
        private float _velocityX = 0f;
        private float _velocityY = 0f;

        public delegate void OnHorizontalDragEvent(float deltaX);
        public delegate void OnVerticalDragEvent(float deltaY);

        public event OnHorizontalDragEvent OnHorizontalDrag;
        public event OnVerticalDragEvent OnVerticalDrag;

        protected override void Initialize()
        {
            GetEssentialComponent(out _rectTransform);
        }

        protected override void Validate()
        {
            if (!ElementContainer)
                FailValidation("Element Container is not assigned.");
        }

        protected override void OnInitializationSuccess()
        {
            FindAllElements();
        }

        private void Update()
        {
            if (IsEmpty() || !IsMovingAllowed())
                return;

            if (IsDragging)
            {
                Vector2 dragDelta = _currentPointerPosition - _lastPointerPosition;

                float deltaX = dragDelta.x;
                float deltaY = dragDelta.y;

                ApplyDragVelocity(dragDelta);

                if (!Mathf.Approximately(deltaX, 0f))
                {
                    MoveElementsHorizontally(deltaX);

                    OnHorizontalDrag?.Invoke(deltaX);
                }

                if (!Mathf.Approximately(deltaY, 0f))
                    OnVerticalDrag?.Invoke(deltaY);

                _lastPointerPosition = _currentPointerPosition;
            }
            else
            {
                ApplyInertiaVelocity();
                MoveElementsHorizontally(_velocityX * Time.deltaTime);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;

            _lastPointerPosition = eventData.position;
            _currentPointerPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _currentPointerPosition = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
        }

        private void FindAllElements()
        {
            ISnekEndlessCarouselElement[] elements = ElementContainer.GetComponentsInChildren<ISnekEndlessCarouselElement>(true);

            _elements = new List<ISnekEndlessCarouselElement>(elements);
        }

        private bool IsEmpty()
        {
            return _elements == null || _elements.Count < 1;
        }

        private bool IsMovingAllowed()
        {
            float totalRequiredElementWidth = _elements.Count * ElementWidth;
            float totalRequiredElementSpacing = (_elements.Count - 1) * ElementSpacing; //spacing is only in-between elements, so its not needed after the last element
            float totalPadding = ElementSpacing * 2f;

            float totalRequiredWidth = totalRequiredElementWidth + totalRequiredElementSpacing + totalPadding;

            return _rectTransform.rect.width - totalRequiredWidth < 0f;
        }

        private void ApplyDragVelocity(Vector2 dragDelta)
        {
            _velocityX = dragDelta.x / Time.deltaTime;
            _velocityY = dragDelta.y / Time.deltaTime;
        }

        private void ApplyInertiaVelocity()
        {
            _velocityX = Mathf.Lerp(_velocityX, 0f, Time.deltaTime * 7.5f);
            _velocityY = Mathf.Lerp(_velocityY, 0f, Time.deltaTime * 7.5f);
        }

        private void MoveElementsHorizontally(float positionOffset)
        {
            float loopBoundOffset = ElementWidth / 2f + ElementSpacing;

            if (positionOffset > 0f)
                MoveElementsRight(positionOffset, ElementContainer.GetTotalWidth() - loopBoundOffset);
            else if (positionOffset < 0f)
                MoveElementsLeft(positionOffset, -loopBoundOffset);
        }

        private void MoveElementsRight(float positionOffset, float loopTriggerPositionX)
        {
            ISnekEndlessCarouselElement loopInitiatorElement = null;

            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                ISnekEndlessCarouselElement element = _elements[i];
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
            ISnekEndlessCarouselElement loopInitiatorElement = null;

            for (int i = 0; i < _elements.Count; i++)
            {
                ISnekEndlessCarouselElement element = _elements[i];
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

        private void LoopElementLeft(ISnekEndlessCarouselElement element)
        {
            RectTransform rectTransform = element.GetRectTransform();
            RectTransform rightMostTransform = _elements[^1].GetRectTransform();

            Vector2 targetPosition = rightMostTransform.anchoredPosition;
            targetPosition.x += ElementWidth + ElementSpacing;

            rectTransform.anchoredPosition = targetPosition;

            _elements.Remove(element);
            _elements.Add(element);
        }

        private void LoopElementRight(ISnekEndlessCarouselElement element)
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
}