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
    [SerializeField] private RectTransform _horizontalLayoutGroupTransform;
    [SerializeField] private RectTransform _textBoxParentTransform;

    private PortfolioProject _project;
    private Action _onPrepareDemoVideo;

    protected override bool IsManuallyInitialized()
    {
        return true;
    }

    public void InitializeExternally(PortfolioProject project, Action onDemoVideoPrepare)
    {
        _project = project;
        _onPrepareDemoVideo = onDemoVideoPrepare;

        RunInitialization();
    }

    protected override void Initialize()
    {
        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_eventManager, nameof(_eventManager));
        ValidateEssentialComponent(_projectName, nameof(_projectName));
        ValidateEssentialComponent(_thumbnail, nameof(_thumbnail));
        ValidateEssentialComponent(_videoDemo, nameof(_videoDemo));
        ValidateEssentialComponent(_descriptionTextBox, nameof(_descriptionTextBox));
        ValidateEssentialComponent(_developmentHighlightsTextBox, nameof(_developmentHighlightsTextBox));
        ValidateEssentialComponent(_scrollRect, nameof(_scrollRect));
        ValidateEssentialComponent(_horizontalLayoutGroupTransform, nameof(_horizontalLayoutGroupTransform));
        ValidateEssentialComponent(_textBoxParentTransform, nameof(_textBoxParentTransform));

        if (_project == null || !_project.IsDataValid())
            FailValidation("Provided project is null or has invalid data, cannot apply data.");

        if (_onPrepareDemoVideo == null)
            FailValidation("Demo video preparation callback not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _thumbnail.sprite = _project.GetThumbnail();

        _projectName.SetText(_project.GetProjectName());
        _videoDemo.InitializeExternally(_project.GetVideoDemoUrl(), OnVideoDemoPrepared);
    }

    protected override void OnFailValidation()
    {
        _eventManager.RequestShowAllProjects();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            _eventManager.RequestShowAllProjects();
    }

    private void OnVideoDemoPrepared()
    {
        _descriptionTextBox.SetText(_project.GetDescriptionText());

        UpdateDevelopmentHighlights();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);

        float targetHeight = Mathf.Max(
            _textBoxParentTransform.rect.height,
            _videoDemo.GetRectTransform().rect.height);

        _horizontalLayoutGroupTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);

        _scrollRect.verticalNormalizedPosition = 1f;

        _onPrepareDemoVideo.Invoke();
    }

    private void UpdateDevelopmentHighlights()
    {
        string developmentHighlights = _project.GetDevelopmentHighlights();
        bool isTextBoxRequired = !string.IsNullOrWhiteSpace(developmentHighlights);

        _developmentHighlightsTextBox.gameObject.SetActive(isTextBoxRequired);

        if (isTextBoxRequired)
            _developmentHighlightsTextBox.SetText(developmentHighlights);
    }
}
