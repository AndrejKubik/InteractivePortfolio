using System;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioMenuController : SnekMonoBehaviour
{
    private EventManager _eventManager;

    [SerializeField] private GameObject _allProjectsMenu;
    [SerializeField] private PortfolioProjectOverview _projectOverviewMenu;

    private GameObject _activeMenu;

    protected override void Initialize()
    {
        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();
    }

    protected override void Validate()
    {
        if (!_eventManager)
            FailValidation("Cannot find event manager singleton.");

        if (!_allProjectsMenu)
            FailValidation("All projects menu not assigned.");

        if (!_projectOverviewMenu)
            FailValidation("Project overview menu not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _eventManager.OnRequestProjectOverview += OnRequestProjectOverview;
        _eventManager.OnRequestShowAllProjects += OnRequestShowAllProjects;

        _activeMenu = _allProjectsMenu;
    }

    private void OnDestroy()
    {
        _eventManager.OnRequestProjectOverview -= OnRequestProjectOverview;
        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;
    }

    private void OnRequestProjectOverview(PortfolioProjectData projectData)
    {
        ShowMenu(_projectOverviewMenu.gameObject);

        _projectOverviewMenu.ApplyProjectData(projectData);
    }

    private void OnRequestShowAllProjects()
    {
        ShowMenu(_allProjectsMenu);
    }

    private void ShowMenu(GameObject targetMenu)
    {
        _activeMenu.SetActive(false);
        
        targetMenu.SetActive(true);

        _activeMenu = targetMenu;
    }
}
