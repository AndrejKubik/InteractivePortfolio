using Snek.EndlessCarousel;
using Snek.GameUI;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioCategory : SnekMonoBehaviour
{
    [SerializeField] private SnekEndlessCarousel _endlessCarousel;

    [Space(10f)]
    [SerializeField] private bool _usedInScrollRect;
    [SerializeField] private ScrollRect _parentScrollRect;
    [SerializeField] private SnekUIPointerInputDispatcher _pointerInputDispatcher;

    private float _parentScrollRectVelocityY;

    protected override void Validate()
    {
        if (!_endlessCarousel)
            FailValidation("Endless carousel not assigned.");

        if (!_usedInScrollRect)
            return;

        if (!_parentScrollRect)
            FailValidation("Parent Scroll Rect is not assigned.");

        if (!_pointerInputDispatcher)
            FailValidation("Pointer input dispatcher is not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _endlessCarousel.OnVerticalDrag += OnEndlessCarouselVerticalDrag;
    }

    private void OnDestroy()
    {
        _endlessCarousel.OnVerticalDrag -= OnEndlessCarouselVerticalDrag;
    }

    private void Update()
    {
        if (_pointerInputDispatcher.IsDragging)
        {
            _parentScrollRectVelocityY = 0f;

            return;
        }

        if (_endlessCarousel.IsDragging)
            return;

        ApplyVerticalInertiaToParentScrollRect();
        MoveParentScrollViewVertically(_parentScrollRectVelocityY * Time.deltaTime);
    }

    protected void OnEndlessCarouselVerticalDrag(float deltaY)
    {
        if (_usedInScrollRect && !_pointerInputDispatcher.IsDragging)
            MoveParentScrollViewVertically(deltaY);
    }

    private void MoveParentScrollViewVertically(float deltaY)
    {
        RectTransform scrollRectContent = _parentScrollRect.content;

        float maxY = Mathf.Max(0f, scrollRectContent.rect.height - _parentScrollRect.viewport.rect.height);

        Vector2 newScrollPosition = scrollRectContent.anchoredPosition;
        newScrollPosition.y = Mathf.Clamp(newScrollPosition.y + deltaY, 0f, maxY);

        scrollRectContent.anchoredPosition = newScrollPosition;

        if (newScrollPosition.y <= 0f || newScrollPosition.y >= maxY)
            _parentScrollRectVelocityY = 0f;
        else
            _parentScrollRectVelocityY = deltaY / Time.deltaTime;
    }

    private void ApplyVerticalInertiaToParentScrollRect()
    {
        _parentScrollRectVelocityY = Mathf.Lerp(_parentScrollRectVelocityY, 0f, Time.deltaTime * 7.5f);
    }
}
