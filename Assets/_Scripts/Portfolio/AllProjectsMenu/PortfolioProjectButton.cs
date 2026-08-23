using System;
using Snek.EndlessCarousel;
using Snek.GameUI;
using Snek.SingletonManager;
using Snek.Utilities;
using TMPro;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectButton : SnekUIButton, ISnekEndlessCarouselElement
{
    private EventManager _eventManager;

    [SerializeField] private GameObject _loadingOverlay;

    [Space(10f)]
    [SerializeField] private PortfolioProjectData _projectData;

    protected override void Initialize()
    {
        base.Initialize();

        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();
    }

    protected override void Validate()
    {
        base.Validate();

        if (!_eventManager)
            FailValidation("Cannot find event manager singleton.");

        ValidateEssentialComponent(_loadingOverlay, nameof(_loadingOverlay));

        if (!_projectData)
            FailValidation("Project data not assigned.");
        else if (!_projectData.IsDataValid())
            FailValidation("Project data contains invalid values.");
    }

    protected override void OnInitializationSuccess()
    {
        GetComponentInChildren<TextMeshProUGUI>(true).text = transform.GetSiblingIndex().ToString();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;
    }

    private void OnRequestShowAllProjects()
    {
        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;

        _loadingOverlay.SetActive(false);
    }

    public RectTransform GetRectTransform()
    {
        return _button.targetGraphic.rectTransform;
    }

    protected override void OnButtonClick()
    {
        _loadingOverlay.SetActive(true);

        _eventManager.OnRequestShowAllProjects += OnRequestShowAllProjects;

        _eventManager.RequestProjectOverview(_projectData);
    }
}
