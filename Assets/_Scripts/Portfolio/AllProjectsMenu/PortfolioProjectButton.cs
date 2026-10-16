using Snek.EndlessCarousel;
using Snek.GameUI;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioProjectButton : SnekUIButton, ISnekEndlessCarouselElement, ISnekInitializableWithData<PortfolioProjectButton.Data>
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

    [SerializeField] private TextMeshProUGUI _projectName;
    [SerializeField] private Image _contentImage;
    [SerializeField] private GameObject _publishedTag;
    [SerializeField] private GameObject _loadingOverlay;

    private EventManager _eventManager;
    private PortfolioProject _project;

    public void PrepareInitializationData(Data data)
    {
        _project = new PortfolioProject(data.ProjectData);
        _eventManager = data.EventManager;
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_projectName, nameof(_projectName));
        ValidateEssentialComponent(_contentImage, nameof(_contentImage));
        ValidateEssentialComponent(_publishedTag, nameof(_publishedTag));
        ValidateEssentialComponent(_loadingOverlay, nameof(_loadingOverlay));
        
        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        
        if (_project == null)
            FailValidation("Portfolio project not created.");
        else if (!_project.IsDataValid())
            FailValidation("Project data contains invalid values.");

        base.Validate();
    }

    protected override void OnInitializationSuccess()
    {
        _contentImage.sprite = _project.GetThumbnail();
        
        _projectName.SetText(_project.GetProjectName());
        _publishedTag.SetActive(_project.IsPublished());

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

        _eventManager.RequestProjectOverview(_project);
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
