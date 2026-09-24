using System;
using Snek.GameUI;
using Snek.SingletonManager;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioProjectOverview : SnekMonoBehaviour, ISnekInitializableExternal<PortfolioProjectOverview.Data>
{
    public readonly struct Data
    {
        public readonly PortfolioProject Project;
        public readonly float SlideDuration;
        public readonly Action OnPrepareDemoVideo;

        public Data(PortfolioProject project, float slideDuration, Action onPrepareDemoVideo)
        {
            Project = project;
            SlideDuration = slideDuration;
            OnPrepareDemoVideo = onPrepareDemoVideo;
        }
    }

    private EventManager _eventManager;

    [SerializeField] private TextMeshProUGUI _projectName;
    [SerializeField] private Image _thumbnail;

    [Space(10f)]
    [SerializeField] private PortfolioProjectVideoDemo _videoDemo;
    [SerializeField] private RectTransform _videoPlayer;
    [SerializeField] private RectTransform _videoPlayerContainerMini;
    [SerializeField] private RectTransform _videoPlayerContainerFullscreen;

    [Space(10f)]
    [SerializeField] private TextBox _descriptionTextBox;
    [SerializeField] private TextBox _developmentHighlightsTextBox;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _horizontalLayoutGroupTransform;
    [SerializeField] private RectTransform _infoSectionTransform;
    [SerializeField] private RectTransform _scrollViewContentTransform;
    [SerializeField] private RectTransform _headerTransform;
    [SerializeField] private SnekUIButton _backToAllProjectsButton;

    private PortfolioProject _project = null;
    private float _slideDuration = 0f;
    private Action _onPrepareDemoVideo = null;

    public void OnBeforeInitialize(Data data)
    {
        _project = data.Project;
        _slideDuration = data.SlideDuration;
        _onPrepareDemoVideo = data.OnPrepareDemoVideo;
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
        ValidateEssentialComponent(_videoPlayer, nameof(_videoPlayer));
        ValidateEssentialComponent(_videoPlayerContainerMini, nameof(_videoPlayerContainerMini));
        ValidateEssentialComponent(_videoPlayerContainerFullscreen, nameof(_videoPlayerContainerFullscreen));

        ValidateEssentialComponent(_descriptionTextBox, nameof(_descriptionTextBox));
        ValidateEssentialComponent(_developmentHighlightsTextBox, nameof(_developmentHighlightsTextBox));
        ValidateEssentialComponent(_scrollRect, nameof(_scrollRect));
        ValidateEssentialComponent(_horizontalLayoutGroupTransform, nameof(_horizontalLayoutGroupTransform));
        ValidateEssentialComponent(_infoSectionTransform, nameof(_infoSectionTransform));
        ValidateEssentialComponent(_scrollViewContentTransform, nameof(_scrollViewContentTransform));
        ValidateEssentialComponent(_headerTransform, nameof(_headerTransform));
        ValidateEssentialComponent(_backToAllProjectsButton, nameof(_backToAllProjectsButton));

        if (_project == null || !_project.IsDataValid())
            FailValidation("Provided project is null or has invalid data, cannot apply data.");

        if (_onPrepareDemoVideo == null)
            FailValidation("Demo video preparation callback not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _thumbnail.sprite = _project.GetThumbnail();

        _projectName.SetText(_project.GetProjectName());

        _videoDemo.InitializeExternally(new PortfolioProjectVideoDemo.Data(
            _project.GetVideoDemoUrl(),
            _slideDuration,
            OnVideoDemoPrepared));

        _backToAllProjectsButton.SetExternalCallback(_eventManager.RequestShowAllProjects);
    }

    private void OnVideoDemoPrepared(VideoAspectForm videoAspectForm)
    {
        switch (videoAspectForm)
        {
            case VideoAspectForm.Landscape:

                _headerTransform.SetParent(_scrollViewContentTransform);
                break;

            case VideoAspectForm.Portrait:

                _headerTransform.SetParent(_infoSectionTransform);
                break;

            default:
            case VideoAspectForm.Unsupported:

                Debug.LogError("Invalid video aspect form provided, cannot prepare video demo.");
                return;
        }

        _headerTransform.SetAsFirstSibling();

        _descriptionTextBox.SetText(_project.GetDescriptionText());

        UpdateDevelopmentHighlights();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollViewContentTransform);

        float targetHeight = Mathf.Max(
            _infoSectionTransform.rect.height,
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

    public void FadeVideoVolume()
    {
        _videoDemo.FadeVolume();
    }
}
