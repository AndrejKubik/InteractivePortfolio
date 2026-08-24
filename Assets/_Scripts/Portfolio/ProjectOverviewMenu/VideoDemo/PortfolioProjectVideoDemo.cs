using System;
using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[UseSnekInspector]
public class PortfolioProjectVideoDemo : SnekMonoBehaviour
{
    private const float VideoVerticalPadding = 15f;

    private LayoutElement _layoutElement;

    private string _videoURL = string.Empty;

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _videoPreview;
    [SerializeField] private RectTransform _videoPreviewHeader;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    [SerializeField] private PortfolioProjectVideoDemoTimeline _videoTimeline;

    [Space(10f)]
    [SerializeField] private PortfolioProjectVideoDemoPlayPauseButton _playPauseControlButton;
    [SerializeField] private PortfolioProjectVideoDemoPlayPauseButton _playPauseOverlayButton;
    [SerializeField] private Sprite _playSymbol;
    [SerializeField] private Sprite _pauseSymbol;

    [Min(0f)]
    [SerializeField] private float _overlayFadeTime = 0.5f;

    private RectTransform _videoPlayerTransform;
    private RectTransform _horizontalLayoutGroupTransform;
    private float _headerHeight = 0f;

    private RenderTexture _renderTexture;
    private Action _onVideoPrepared;

    private float _videoTotalTime = 0f;
    private float _videoProgress = 0f;

    private float _playPauseOverlaySymbolAlpha = 0f;

    protected override bool IsManuallyInitialized()
    {
        return true;
    }

    public void InitializeExternally(string videoURL, Action onVideoPrepared = null)
    {
        _videoURL = videoURL;
        _onVideoPrepared = onVideoPrepared;

        RunInitialization();
    }

    protected override void Initialize()
    {
        GetEssentialComponent(out _layoutElement);
    }

    protected override void Validate()
    {
        if (string.IsNullOrEmpty(_videoURL))
            FailValidation("Invalid video URL provided.");

        ValidateEssentialComponent(_videoPlayer, nameof(_videoPlayer));
        ValidateEssentialComponent(_videoPreview, nameof(_videoPreview));
        ValidateEssentialComponent(_videoPreviewHeader, nameof(_videoPreviewHeader));
        ValidateEssentialComponent(_aspectRatioFitter, nameof(_aspectRatioFitter));
        ValidateEssentialComponent(_videoTimeline, nameof(_videoTimeline));

        ValidateEssentialComponent(_playPauseControlButton, nameof(_playPauseControlButton));
        ValidateEssentialComponent(_playPauseOverlayButton, nameof(_playPauseOverlayButton));
        ValidateEssentialComponent(_playSymbol, nameof(_playSymbol));
        ValidateEssentialComponent(_pauseSymbol, nameof(_pauseSymbol));
    }

    protected override void OnInitializationSuccess()
    {
        _videoPlayerTransform = _videoPlayer.transform as RectTransform;
        _horizontalLayoutGroupTransform = _horizontalLayoutGroup.transform as RectTransform;

        _headerHeight = _videoPreviewHeader.rect.size.y;

        _videoPlayer.url = _videoURL;
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        _videoTimeline.InitializeExternally(OnUserMoveTimeline);

        _playPauseControlButton.SetExternalCallback(OnPlayPauseButtonClick);
        _playPauseControlButton.SetSymbol(_pauseSymbol);

        _playPauseOverlayButton.SetExternalCallback(OnPlayPauseButtonClick);
        _playPauseOverlayButton.SetSymbol(_playSymbol);

        _videoPlayer.Prepare();
    }

    private void OnDestroy()
    {
        if (_isValid)
            _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    private void Update()
    {
        _videoProgress = Mathf.InverseLerp(0f, _videoTotalTime, (float)_videoPlayer.time);

        _videoTimeline.SetValue(_videoProgress, false);

        if(_playPauseOverlaySymbolAlpha > 0f)
            FadePlayPauseOverlaySymbol();
    }

    private void FadePlayPauseOverlaySymbol()
    {
        _playPauseOverlaySymbolAlpha = Mathf.MoveTowards(
            _playPauseOverlaySymbolAlpha,
            0f,
            Time.deltaTime / _overlayFadeTime);

        _playPauseOverlayButton.SetSymbolAlpha(_playPauseOverlaySymbolAlpha);
    }

    private void OnPlayPauseButtonClick()
    {
        if (_videoPlayer.isPaused)
        {
            _videoPlayer.Play();
            _playPauseControlButton.SetSymbol(_pauseSymbol);
            _playPauseOverlayButton.SetSymbol(_playSymbol);
        }
        else
        {
            _videoPlayer.Pause();
            _playPauseControlButton.SetSymbol(_playSymbol);
            _playPauseOverlayButton.SetSymbol(_pauseSymbol);
        }

        ShowPlayPauseOverlaySymbol();
    }

    private void ShowPlayPauseOverlaySymbol()
    {
        _playPauseOverlaySymbolAlpha = 1f;

        _playPauseOverlayButton.SetSymbolAlpha(_playPauseOverlaySymbolAlpha);
    }

    private void OnUserMoveTimeline(float newTime)
    {
        _videoPlayer.time = Mathf.Lerp(0f, _videoTotalTime, newTime);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        CreateAndApplyRenderTexture(source);
        ApplyAspectRatioToVideoRectSize(source);
        FitVideoPreviewToScreen();

        _videoTotalTime = (float)_videoPlayer.length;

        _videoPlayer.Play();

        _onVideoPrepared?.Invoke();
    }

    private void CreateAndApplyRenderTexture(VideoPlayer source)
    {
        _renderTexture = new RenderTexture((int)source.width, (int)source.height, 0);

        _renderTexture.Create();

        _videoPlayer.targetTexture = _renderTexture;
        _videoPreview.texture = _renderTexture;
    }

    private void ApplyAspectRatioToVideoRectSize(VideoPlayer source)
    {
        _aspectRatioFitter.enabled = false;

        _aspectRatioFitter.aspectRatio = (float)source.width / (float)source.height;

        _aspectRatioFitter.aspectMode = _aspectRatioFitter.aspectRatio > 1f ?
            AspectRatioFitter.AspectMode.WidthControlsHeight : AspectRatioFitter.AspectMode.HeightControlsWidth;

        _aspectRatioFitter.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_aspectRatioFitter.GetComponent<RectTransform>());
    }

    private void FitVideoPreviewToScreen()
    {
        ResetHorizontalLayoutGroupHeight();

        switch (_aspectRatioFitter.aspectMode)
        {
            case AspectRatioFitter.AspectMode.WidthControlsHeight:

                FitVideoPreviewToScreenLandscape();
                break;

            case AspectRatioFitter.AspectMode.HeightControlsWidth:

                FitVideoPreviewToScreenPortrait();
                break;

            default:

                Debug.LogError("Unsupported aspect mode provided, cannot fit video preview to screen.", gameObject);
                break;
        }
    }

    private void ResetHorizontalLayoutGroupHeight()
    {
        _horizontalLayoutGroupTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);
    }

    private void FitVideoPreviewToScreenLandscape()
    {
        float targetWidth = _horizontalLayoutGroupTransform.rect.size.x / 2f;
        targetWidth -= 2f * VideoVerticalPadding;

        _layoutElement.preferredWidth = targetWidth;

        _videoPlayerTransform.ResetAnchorOffset();
        _videoPlayerTransform.SetAnchorOffset(_headerHeight, AnchorOffsetSide.Top);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);
    }

    private void FitVideoPreviewToScreenPortrait()
    {
        float targetHeight = (float)Screen.height - 2f * VideoVerticalPadding;
        targetHeight -= _headerHeight;

        _horizontalLayoutGroupTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);

        _layoutElement.preferredWidth = _videoPlayerTransform.rect.size.x;

        _videoPlayerTransform.ResetAnchorOffset();
        _videoPlayerTransform.SetAnchorOffset(_headerHeight, AnchorOffsetSide.Top);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);
    }

    public RectTransform GetRectTransform()
    {
        return transform as RectTransform;
    }
}
