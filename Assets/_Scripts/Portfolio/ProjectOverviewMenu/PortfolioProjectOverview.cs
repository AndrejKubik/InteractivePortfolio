using System;
using Snek.SingletonManager;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioProjectOverview : SnekMonoBehaviour
{
    private EventManager _eventManager;

    [SerializeField] private TextMeshProUGUI _projectName;
    [SerializeField] private Image _thumbnail;
    [SerializeField] private PortfolioProjectVideoDemo _videoDemo;
    [SerializeField] private TextBox _descriptionTextBox;
    [SerializeField] private TextBox _developmentHighlightsTextBox;
    [SerializeField] private ScrollRect _scrollRect;

    private PortfolioProjectData _projectData;

    protected override bool IsManuallyInitialized()
    {
        return true;
    }

    public void InitializeExternally(PortfolioProjectData projectData)
    {
        _projectData = projectData;

        RunInitialization();
    }

    protected override void Initialize()
    {
        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();
    }

    protected override void Validate()
    {
        if (_projectData == null || !_projectData.IsDataValid())
        {
            _eventManager.RequestShowAllProjects();

            FailValidation("Provided project data is null or has invalid values, cannot apply data.");
        }

        if (!_eventManager)
            FailValidation("Cannot find event manager singleton.");

        if (!_projectName)
            FailValidation("Project name text mesh not assigned.");

        if (!_thumbnail)
            FailValidation("Thumbnail not assigned.");

        if (!_videoDemo)
            FailValidation("Video demo not assigned.");

        if (!_descriptionTextBox)
            FailValidation("Description text mesh not assigned.");

        if (!_developmentHighlightsTextBox)
            FailValidation("Development highlights not assigned.");

        if (!_scrollRect)
            FailValidation("Scroll rect not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _thumbnail.sprite = _projectData.GetThumbnail();
        _projectName.SetText(_projectData.GetProjectName());

        _videoDemo.Initialize(_projectData.GetVideoDemoLink(), OnVideoDemoPrepared);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            _eventManager.RequestShowAllProjects();
    }

    private void OnVideoDemoPrepared()
    {
        _descriptionTextBox.SetText(_projectData.GetDescriptionText());
        _developmentHighlightsTextBox.SetText(_projectData.GetDevelopmentHighlights());

        _scrollRect.verticalNormalizedPosition = 1f;
    }
}
