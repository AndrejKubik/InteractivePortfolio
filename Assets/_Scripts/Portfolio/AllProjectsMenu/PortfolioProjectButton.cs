using Snek.EndlessCarousel;
using Snek.GameUI;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PortfolioProjectButtonTagController))]
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

    private enum ButtonType
    {
        SmallButton,
        BigButton
    }

    [SerializeField] private TextMeshProUGUI _projectName;
    [SerializeField] private Image _contentImage;
    [SerializeField] private GameObject _publishedTag;
    [SerializeField] private GameObject _prototypeTag;
    [SerializeField] private GameObject _loadingOverlay;

    [Space(10f)]
    [SerializeField] private ButtonType _buttonType = ButtonType.SmallButton;

    private EventManager _eventManager;
    private PortfolioProjectButtonTagController _tagController;
    private PortfolioProject _project;

    public void PrepareInitializationData(Data data)
    {
        _project = new PortfolioProject(data.ProjectData);
        _eventManager = data.EventManager;
    }

    protected override void OnInitialize()
    {
        GetEssentialComponent(out _tagController);

        base.OnInitialize();
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_projectName, nameof(_projectName));
        ValidateEssentialComponent(_contentImage, nameof(_contentImage));
        ValidateEssentialComponent(_publishedTag, nameof(_publishedTag));
        ValidateEssentialComponent(_prototypeTag, nameof(_prototypeTag));
        ValidateEssentialComponent(_loadingOverlay, nameof(_loadingOverlay));

        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        ValidateEssentialComponent(_tagController, nameof(_tagController));

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
        _prototypeTag.SetActive(!_project.IsPublished());

        CreateProjectTags();

        base.OnInitializationSuccess();
    }

    protected override void OnDispose()
    {
        _eventManager.OnRequestShowAllProjects -= OnRequestShowAllProjects;

        base.OnDispose();
    }

    protected override void OnButtonClick()
    {
        _loadingOverlay.SetActive(true);

        _eventManager.OnRequestShowAllProjects += OnRequestShowAllProjects;

        _eventManager.RequestProjectOverview(_project);
    }

    private void CreateProjectTags()
    {
        var tagsList = _buttonType switch
        {
            ButtonType.SmallButton => _project.GetSmallButtonTags(),
            ButtonType.BigButton => _project.GetBigButtonTags(),
            _ => null
        };

        if(tagsList == null)
        {
            Debug.LogError("Invalid button type assigned, cannot create tags.");

            return;
        }

        foreach (string tag in tagsList)
            _tagController.CreateTag(tag);
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
