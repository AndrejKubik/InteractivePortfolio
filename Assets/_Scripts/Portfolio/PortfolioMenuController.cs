using System;
using DG.Tweening;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioMenuController : SnekMonoBehaviour
{
    private EventManager _eventManager;

    [SerializeField] private GameObject _allProjectsMenu;
    [SerializeField] private PortfolioProjectOverview _projectOverviewMenu;
    [SerializeField] private RectTransform _backgroundTransform;

    [Space(10f)]
    [Min(0f)]
    [SerializeField] private float _menuSlideDuration = 0.5f;

    protected override void Initialize()
    {
        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        ValidateEssentialComponent(_allProjectsMenu, nameof(_allProjectsMenu));
        ValidateEssentialComponent(_projectOverviewMenu, nameof(_projectOverviewMenu));
        ValidateEssentialComponent(_backgroundTransform, nameof(_backgroundTransform));
    }

    protected override void OnInitializationSuccess()
    {
        _eventManager.OnRequestProjectOverview += OnRequestProjectOverview;
        _eventManager.OnRequestShowAllProjects += OnRequestShowAllProjects;
    }

    private void OnDestroy()
    {
        _eventManager.OnRequestProjectOverview -= OnRequestProjectOverview;
        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;
    }

    private void OnRequestProjectOverview(PortfolioProjectData projectData)
    {
        ShowProjectOverviewMenu(true);

        _projectOverviewMenu.InitializeExternally(projectData);
    }

    private void OnRequestShowAllProjects()
    {
        ShowProjectOverviewMenu(false);
    }

    private void ShowProjectOverviewMenu(bool newState)
    {
        var menuTransform = _projectOverviewMenu.transform as RectTransform;
        float backgroundWidth = _backgroundTransform.rect.width;

        if (newState == true)
        {
            menuTransform.anchoredPosition = new Vector2(backgroundWidth, 0f);

            _projectOverviewMenu.gameObject.SetActive(true);

            SlideTransformHorizontally(menuTransform, 0f)
                .OnComplete(OnProjectOverviewSlideIn);
        }
        else if (newState == false)
        {
            _allProjectsMenu.SetActive(true);

            SlideTransformHorizontally(menuTransform, backgroundWidth)
                .OnComplete(OnProjectOverviewSlideOut);
        }
    }

    private void OnProjectOverviewSlideIn()
    {
        _allProjectsMenu.SetActive(false);
    }

    private void OnProjectOverviewSlideOut()
    {
        _projectOverviewMenu.gameObject.SetActive(false);
    }

    private Tween SlideTransformHorizontally(RectTransform rectTransform, float targetPositionX)
    {
        rectTransform.DOKill();

        return rectTransform
            .DOAnchorPosX(targetPositionX, _menuSlideDuration)
            .SetEase(Ease.OutCubic);
    }
}
