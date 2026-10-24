using System;
using System.Collections.Generic;
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

    [Serializable]
    private struct DevelopmentStateTagData
    {
        public PortfolioProjectDevelopmentState DevelopmentState;
        public Color Color;
        public string Text;
    }

    private enum ButtonType
    {
        SmallButton,
        BigButton
    }

    [SerializeField] private TextMeshProUGUI _projectName;
    [SerializeField] private Image _contentImage;
    [SerializeField] private PortfolioProjectButtonTag _developmentStateTag;
    [SerializeField] private GameObject _loadingOverlay;

    [Space(10f)]
    [SerializeField] private ButtonType _buttonType = ButtonType.SmallButton;

    [Space(10f)]
    [SerializeField] private List<DevelopmentStateTagData> _developmentStateTagDataCombinations = new();

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
        if (_project == null)
            FailValidation("Portfolio project not created.");
        else if (!_project.IsDataValid())
            FailValidation("Project data contains invalid values.");

        ValidateEssentialComponent(_projectName, nameof(_projectName));
        ValidateEssentialComponent(_contentImage, nameof(_contentImage));
        ValidateEssentialComponent(_developmentStateTag, nameof(_developmentStateTag));
        ValidateEssentialComponent(_loadingOverlay, nameof(_loadingOverlay));

        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        ValidateEssentialComponent(_tagController, nameof(_tagController));

        if (!IsEveryDevelopmentStatePopulatedWithTagData())
            FailValidation(
                $"{nameof(_developmentStateTagDataCombinations)} list is missing development states or has duplicates.\n" +
                $"Make sure every development state tag data has a unique development state value.");
        else if (!IsEveryDevelopmentStateTagDataValid())
            FailValidation("Some development state tag data has completely transparent color or empty text.");

        base.Validate();
    }

    protected override void OnInitializationSuccess()
    {
        _contentImage.sprite = _project.GetThumbnail();

        _projectName.SetText(_project.GetProjectName());

        CreateProjectTags();
        UpdateDevelopmentStateTag();

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

    private bool IsEveryDevelopmentStatePopulatedWithTagData()
    {
        Array developmentStateValues = Enum.GetValues(typeof(PortfolioProjectDevelopmentState));

        if (_developmentStateTagDataCombinations.Count != developmentStateValues.Length)
            return false;

        foreach (PortfolioProjectDevelopmentState state in developmentStateValues)
        {
            int foundElements = 0;

            foreach (DevelopmentStateTagData tagData in _developmentStateTagDataCombinations)
                if (tagData.DevelopmentState == state)
                    foundElements++;

            if (foundElements != 1)
                return false;
        }

        return true;
    }

    private bool IsAnyDevelopmentStateTagDataWithState(PortfolioProjectDevelopmentState state, out int elementIndex)
    {
        elementIndex = _developmentStateTagDataCombinations.FindIndex(IsDevelopmentStateTagDataWithState);

        return elementIndex != -1;

        bool IsDevelopmentStateTagDataWithState(DevelopmentStateTagData tagData)
        {
            return tagData.DevelopmentState == state;
        }
    }

    private bool IsEveryDevelopmentStateTagDataValid()
    {
        foreach (DevelopmentStateTagData tagData in _developmentStateTagDataCombinations)
            if (!IsDevelopmentStateTagDataValid(tagData))
                return false;

        return true;
    }

    private bool IsDevelopmentStateTagDataValid(DevelopmentStateTagData data)
    {
        return data.Color != default && !string.IsNullOrWhiteSpace(data.Text);
    }

    private void CreateProjectTags()
    {
        var tagsList = _buttonType switch
        {
            ButtonType.SmallButton => _project.GetSmallButtonTags(),
            ButtonType.BigButton => _project.GetBigButtonTags(),
            _ => null
        };

        if (tagsList == null)
        {
            Debug.LogError("Invalid button type assigned, cannot create tags.");

            return;
        }

        foreach (string tag in tagsList)
            _tagController.CreateTag(tag);
    }

    private void UpdateDevelopmentStateTag()
    {
        PortfolioProjectDevelopmentState developmentState = _project.GetDevelopmentState();

        if (!IsAnyDevelopmentStateTagDataWithState(developmentState, out int elementIndex))
        {
            Debug.LogError(
                $"No development state tag data found with state: {developmentState}\n" +
                $"Cannot update development state tag.");

            return;
        }

        DevelopmentStateTagData tagData = _developmentStateTagDataCombinations[elementIndex];

        _developmentStateTag.SetColor(tagData.Color);
        _developmentStateTag.SetText(tagData.Text);
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
