using Snek.EndlessCarousel;
using Snek.GameUI;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioProjectButton : SnekUIButton, ISnekEndlessCarouselElement, ISnekInitializableExternal<PortfolioProjectButton.Data>
{
    public readonly struct Data
    {
        public readonly PortfolioProjectData ProjectData;
        public readonly EventManager EventManager;

        public Data(PortfolioProjectData projectData, EventManager eventManager)
        {
            ProjectData = projectData;
            EventManager = eventManager;
        }
    }

    private Image _image;

    [SerializeField] private GameObject _loadingOverlay;

    private EventManager _eventManager;
    private PortfolioProjectData _projectData;

    public void OnBeforeInitialize(Data data)
    {
        _projectData = data.ProjectData;
        _eventManager = data.EventManager;
    }

    protected override void Initialize()
    {
        base.Initialize();

        GetEssentialComponent(out _image);
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_loadingOverlay, nameof(_loadingOverlay));
        
        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        
        if (!_projectData)
            FailValidation("Project data not assigned.");
        else if (!_projectData.IsDataValid())
            FailValidation("Project data contains invalid values.");

        base.Validate();
    }

    protected override void OnInitializationSuccess()
    {
        _image.sprite = _projectData.GetThumbnail();

        base.OnInitializationSuccess();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;
    }

    protected override void OnButtonClick()
    {
        _loadingOverlay.SetActive(true);

        _eventManager.OnRequestShowAllProjects += OnRequestShowAllProjects;

        _eventManager.RequestProjectOverview(_projectData);
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
}
