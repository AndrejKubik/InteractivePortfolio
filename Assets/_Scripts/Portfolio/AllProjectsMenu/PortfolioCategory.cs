using System.Collections.Generic;
using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioCategory : SnekMonoBehaviour
{
    [SerializeField] private PortfolioCategoryCarouselController _projectButtonsCarouselController;
    [SerializeField] private PortfolioProjectButton _projectButtonPrefab;

    [Space(10f)]
    [SerializeField] private bool _usedInScrollRect;
    [SerializeField] private ScrollRect _parentScrollRect;
    [SerializeField] private SnekUIPointerInputDispatcher _pointerInputDispatcher;

    [Space(10f)]
    [SerializeField] private List<PortfolioProjectData> _projects = new();

    private float _parentScrollRectVelocityY;

    protected override void Validate()
    {
        ValidateEssentialComponent(_projectButtonsCarouselController, nameof(_projectButtonsCarouselController));
        ValidateEssentialComponent(_projectButtonPrefab, nameof(_projectButtonPrefab));

        if (_projects.Count < 1)
            FailValidation("No projects assigned to the category.");

        if (!_usedInScrollRect)
            return;
        
        ValidateEssentialComponent(_parentScrollRect, nameof(_parentScrollRect));
        ValidateEssentialComponent(_pointerInputDispatcher, nameof(_pointerInputDispatcher));
    }

    protected override void OnInitializationSuccess()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform); //enforces correct rect calculations down the line

        _projectButtonsCarouselController.InitializeExternally(new PortfolioCategoryCarouselController.Data(
            _projects,
            OnEndlessCarouselVerticalDrag,
            _projectButtonPrefab));
    }

    private void Update()
    {
        if (_pointerInputDispatcher.IsDragging)
        {
            _parentScrollRectVelocityY = 0f;

            return;
        }

        if (_projectButtonsCarouselController.IsDraggingCarousel())
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
