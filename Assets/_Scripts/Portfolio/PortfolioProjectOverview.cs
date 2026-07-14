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
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private PortfolioDevelopmentHighlightsTextBox _developmentHighlightsTextBox;
    [SerializeField] private ScrollRect _scrollRect;

    protected override void Initialize()
    {
        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();
    }

    protected override void Validate()
    {
        if (!_eventManager)
            FailValidation("Cannot find event manager singleton.");

        if (!_projectName)
            FailValidation("Project name text mesh not assigned.");

        if (!_description)
            FailValidation("Description text mesh not assigned.");

        if (!_developmentHighlightsTextBox)
            FailValidation("Development highlights not assigned.");

        if (!_scrollRect)
            FailValidation("Scroll rect not assigned.");
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            _eventManager.RequestShowAllProjects();
    }

    public void ApplyProjectData(PortfolioProjectData projectData)
    {
        if(projectData == null || !projectData.IsDataValid())
        {
            Debug.LogError("Provided project data is null or has invalid values, cannot apply data.");

            _eventManager.RequestShowAllProjects();

            gameObject.SetActive(false);

            return;
        }

        _projectName.SetText(projectData.GetProjectName());
        _thumbnail.sprite = projectData.GetThumbnail();
        _description.SetText(projectData.GetDescriptionText());

        Canvas.ForceUpdateCanvases();

        _developmentHighlightsTextBox.ApplyData(
            projectData.GetDevelopmentHighlights(),
            _description.fontSize);

        _scrollRect.verticalNormalizedPosition = 1f;
    }
}
