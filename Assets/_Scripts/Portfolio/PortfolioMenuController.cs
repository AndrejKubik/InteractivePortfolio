using System;
using DG.Tweening;
using Snek.AudioManager;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioMenuController : SnekMonoBehaviour
{
    private EventManager _eventManager;
    private SnekMusicManager _musicManager;

    [SerializeField] private GameObject _allProjectsMenu;
    [SerializeField] private PortfolioProjectOverview _projectOverviewMenu;
    [SerializeField] private RectTransform _backgroundTransform;

    [Space(10f)]
    [Min(0f)]
    [SerializeField] private float _menuSlideDuration = 0.5f;

    private RectTransform _projectOverviewMenuTransform;
    private float _backgroundWidth = 0f;

    protected override void Initialize()
    {
        SnekSingletonManager.GetSingleton(out _eventManager);
        SnekSingletonManager.GetSingleton(out _musicManager);
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        ValidateEssentialComponent(_musicManager, nameof(_musicManager));

        ValidateEssentialComponent(_allProjectsMenu, nameof(_allProjectsMenu));
        ValidateEssentialComponent(_projectOverviewMenu, nameof(_projectOverviewMenu));
        ValidateEssentialComponent(_backgroundTransform, nameof(_backgroundTransform));
    }

    protected override void OnInitializationSuccess()
    {
        _eventManager.OnRequestProjectOverview += OnRequestProjectOverview;
        _eventManager.OnRequestShowAllProjects += OnRequestShowAllProjects;

        _musicManager.StartPlaylist();

        _projectOverviewMenuTransform = _projectOverviewMenu.transform as RectTransform;
        _backgroundWidth = _backgroundTransform.rect.width;
    }

    private void OnDestroy()
    {
        _eventManager.OnRequestProjectOverview -= OnRequestProjectOverview;
        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;
    }

    private void OnRequestProjectOverview(PortfolioProjectData projectData)
    {
        ShowProjectOverviewMenu(projectData);
    }

    private void ShowProjectOverviewMenu(PortfolioProjectData projectData)
    {
        _projectOverviewMenuTransform.anchoredPosition = new Vector2(_backgroundWidth, 0f);
        _projectOverviewMenu.gameObject.SetActive(true);

        _projectOverviewMenu.InitializeExternally(projectData, OnPrepareProjectOverviewDemoVideo);
    }

    private void OnPrepareProjectOverviewDemoVideo()
    {
        SlideTransformHorizontally(_projectOverviewMenuTransform, 0f)
            .OnComplete(OnProjectOverviewSlideIn);
    }

    private void OnRequestShowAllProjects()
    {
        _allProjectsMenu.SetActive(true);

        HideProjectOverviewMenu();
    }

    private void HideProjectOverviewMenu()
    {
        SlideTransformHorizontally(_projectOverviewMenuTransform, _backgroundWidth)
            .OnComplete(OnProjectOverviewSlideOut);
    }

    private Tween SlideTransformHorizontally(RectTransform rectTransform, float targetPositionX)
    {
        rectTransform.DOKill();

        return rectTransform
            .DOAnchorPosX(targetPositionX, _menuSlideDuration)
            .SetEase(Ease.OutCubic);
    }

    private void OnProjectOverviewSlideIn()
    {
        _allProjectsMenu.SetActive(false);
    }

    private void OnProjectOverviewSlideOut()
    {
        _projectOverviewMenu.gameObject.SetActive(false);
    }
}
