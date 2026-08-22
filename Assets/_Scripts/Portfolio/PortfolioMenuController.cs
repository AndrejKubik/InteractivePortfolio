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

        if(newState == true)
        {
            menuTransform.anchoredPosition = new Vector2(Screen.width, 0f);

            _projectOverviewMenu.gameObject.SetActive(true);

            menuTransform
                .DOAnchorPosX(0f, 0.5f)
                .SetEase(Ease.OutCubic)
                .OnComplete(OnProjectOverviewSlideIn);
        }
        else
        {
            _allProjectsMenu.SetActive(true);

            menuTransform
                .DOAnchorPosX(Screen.width, 0.5f)
                .SetEase(Ease.OutCubic)
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
}
