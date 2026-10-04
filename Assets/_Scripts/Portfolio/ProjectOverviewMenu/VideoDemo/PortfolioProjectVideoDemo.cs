using System;
using DG.Tweening;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(PortfolioProjectVideoPlayerController))]
[RequireComponent(typeof(PortfolioProjectVideoPreviewController))]
[RequireComponent(typeof(PortfolioProjectVideoHoverOverlayController))]
[RequireComponent(typeof(PortfolioProjectVideoControlsPanelController))]
[UseSnekInspector]
public class PortfolioProjectVideoDemo : SnekMonoBehaviour, ISnekInitializableWithData<PortfolioProjectVideoDemo.Data>
{
    public readonly struct Data
    {
        public readonly string VideoUrl;
        public readonly float SlideDuration;
        public readonly Action<VideoAspectForm> OnVideoPrepared;

        public Data(string videoUrl, float slideDuration, Action<VideoAspectForm> onVideoPrepared)
        {
            VideoUrl = videoUrl;
            SlideDuration = slideDuration;
            OnVideoPrepared = onVideoPrepared;
        }
    }

    [Space(10f)]
    [SerializeField] private VideoPlayer _videoPlayer;

    [Space(10f)]
    [SerializeField] private Sprite _mutedSymbol;
    [SerializeField] private Sprite _unmutedSymbol;

    [Space(10f)]
    [SerializeField] private Sprite _playSymbol;
    [SerializeField] private Sprite _pauseSymbol;

    [Space(10f)]
    [SerializeField] private Sprite _fullscreenOnSymbol;
    [SerializeField] private Sprite _fullscreenOffSymbol;

    private string _videoURL = string.Empty;
    private float _slideDuration = 0f;
    private Action<VideoAspectForm> _onVideoPrepared = null;

    private PortfolioProjectVideoPlayerController _videoPlayerController;
    private PortfolioProjectVideoPreviewController _videoPreviewController;
    private PortfolioProjectVideoHoverOverlayController _videoHoverOverlayController;
    private PortfolioProjectVideoControlsPanelController _videoControlsPanelController;

    private Tween _activeVolumeTween = null;

    private float _savedVolume = 0f;
    private bool _savedMuteState = false;

    public bool IsFullscreen { get; private set; }

    private bool _isPlayPauseButtonClicked = false;

    protected override SnekMonoBehaviour[] GetSubdependencies()
    {
        return new SnekMonoBehaviour[]
        {
            _videoPlayerController,
            _videoPreviewController,
            _videoHoverOverlayController,
            _videoControlsPanelController
        };
    }

    public void PrepareInitializationData(Data data)
    {
        _videoURL = data.VideoUrl;
        _slideDuration = data.SlideDuration;
        _onVideoPrepared = data.OnVideoPrepared;
    }

    protected override void OnInitialize()
    {
        GetEssentialComponent(out _videoPlayerController);
        GetEssentialComponent(out _videoPreviewController);
        GetEssentialComponent(out _videoHoverOverlayController);
        GetEssentialComponent(out _videoControlsPanelController);
    }

    protected override void Validate()
    {
        if (string.IsNullOrEmpty(_videoURL))
            FailValidation("Invalid video URL provided.");

        if (_onVideoPrepared == null)
            FailValidation("Video prepared callback not assigned.");

        ValidateEssentialComponent(_videoPlayer, nameof(_videoPlayer));

        ValidateEssentialComponent(_mutedSymbol, nameof(_mutedSymbol));
        ValidateEssentialComponent(_unmutedSymbol, nameof(_unmutedSymbol));
        ValidateEssentialComponent(_playSymbol, nameof(_playSymbol));
        ValidateEssentialComponent(_pauseSymbol, nameof(_pauseSymbol));
        ValidateEssentialComponent(_fullscreenOnSymbol, nameof(_fullscreenOnSymbol));
        ValidateEssentialComponent(_fullscreenOffSymbol, nameof(_fullscreenOffSymbol));
    }

    protected override void OnInitializeSubdependencies()
    {
        _videoPlayerController.Initialize(new PortfolioProjectVideoPlayerController.Data(
            _videoURL,
            OnVideoPrepared));

        _videoPreviewController.Initialize();

        _videoHoverOverlayController.Initialize(new PortfolioProjectVideoHoverOverlayController.Data(
            OnPlayPauseButtonClick,
            OnShowHoverOverlay));

        _videoControlsPanelController.Initialize(new PortfolioProjectVideoControlsPanelController.Data(
            _videoPlayerController.TrySeekVideo,
            SetAudioVolume,
            ToggleAudioMuteMode,
            ToggleFullscreenMode,
            OnPlayPauseButtonClick));
    }

    protected override void OnInitializationSuccess()
    {
        IsFullscreen = false;

        LoadAudioSettings();
    }

    private void Update()
    {
        if (IsVideoTimelineAutoUpdateAllowed())
            _videoControlsPanelController.SetTimelineProgress(_videoPlayerController.GetVideoProgress());

        if (IsFullscreen)
        {
            _videoHoverOverlayController.ControlHoverOverlayUserActivityBased(IsHoverOverlayFadeAllowed());
            _videoControlsPanelController.ControllFullscreenControlsPanelAnimation();
        }
        else
            _videoHoverOverlayController.ControlHoverOverlayConstant();

        if (_isPlayPauseButtonClicked)
            _isPlayPauseButtonClicked = false;
    }

    private bool IsVideoTimelineAutoUpdateAllowed()
    {
        return !_videoPlayerController.IsVideoSeeking && !_videoControlsPanelController.IsTimelineSliderHandleHeld();
    }

    private bool IsHoverOverlayFadeAllowed()
    {
        return !_videoControlsPanelController.IsFullscreenControlsPanelVisible() && !_isPlayPauseButtonClicked;
    }

    private void OnShowHoverOverlay()
    {
        if (IsFullscreen)
            _videoControlsPanelController.ShowFullscreenControlPanel();
    }

    private void LoadAudioSettings()
    {
        _savedVolume = PlayerPrefs.GetFloat(SaveKeys.VideoDemoVolume, 1f);
        _savedMuteState = Convert.ToBoolean(PlayerPrefs.GetInt(SaveKeys.VideoDemoMute, 0));

        _videoControlsPanelController.SetAudioVolume(_savedVolume);

        SetAudioMute(_savedMuteState);
    }

    public void FadeVolume()
    {
        StartFadeVolumeTween(_videoControlsPanelController.GetAudioVolume(), 0f);
    }

    private void StartFadeVolumeTween(float startValue, float endValue)
    {
        if (_activeVolumeTween != null)
            _activeVolumeTween.Kill();

        _activeVolumeTween = DOVirtual.Float(startValue, endValue, _slideDuration, SetAudioVolume);
    }

    private void OnPlayPauseButtonClick()
    {
        if (_videoPlayerController.IsVideoPaused())
        {
            _videoPlayerController.PlayVideo();

            _videoControlsPanelController.SetPlayPauseButtonSymbol(_pauseSymbol);
            _videoHoverOverlayController.ShowFadingOverlay(_playSymbol);
        }
        else
        {
            _videoPlayerController.PauseVideo();

            _videoControlsPanelController.SetPlayPauseButtonSymbol(_playSymbol);
            _videoHoverOverlayController.ShowFadingOverlay(_pauseSymbol);
        }

        _isPlayPauseButtonClicked = true;

        if (IsFullscreen)
            _videoControlsPanelController.ShowFullscreenControlPanel();
    }

    private void SetAudioVolume(float newValue)
    {
        _videoPlayerController.SetAudioVolume(newValue);
        _videoControlsPanelController.MatchMuteAudioButtonSymbolWithVolume();

        SetAudioMute(false);
    }

    private void ToggleAudioMuteMode()
    {
        bool newState = !_videoPlayerController.IsAudioMuted();

        SetAudioMute(newState);

        if (newState == true)
            _videoHoverOverlayController.ShowFadingOverlay(_mutedSymbol);
        else
            _videoHoverOverlayController.ShowFadingOverlay(_unmutedSymbol);
    }

    public void ToggleFullscreenMode()
    {
        IsFullscreen = !IsFullscreen;

        if (IsFullscreen)
        {
            _videoPreviewController.ActivateFullscreenPreview();
            _videoControlsPanelController.ShowFullscreenControlPanel();
        }
        else
        {
            _videoPreviewController.ActivateMiniPreview();
            _videoControlsPanelController.MoveControlsPanelToMiniPlayerDefaultPosition();
        }

        Sprite fullscreenButtonSymbol = IsFullscreen ?
            _fullscreenOffSymbol : _fullscreenOnSymbol;

        _videoControlsPanelController.SetToggleFullscreenButtonSymbol(fullscreenButtonSymbol);
    }

    private void SetAudioMute(bool newState)
    {
        _videoPlayerController.SetAudioMute(newState);

        if (newState == true)
            _videoControlsPanelController.SetMuteAudioButtonMutedSymbol();
        else
            _videoControlsPanelController.MatchMuteAudioButtonSymbolWithVolume();

        PlayerPrefs.SetInt(SaveKeys.VideoDemoMute, Convert.ToInt32(newState));
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        RenderTexture renderTexture = CreateRenderTexture((int)source.width, (int)source.height);

        _videoPlayer.targetTexture = renderTexture;

        _videoPreviewController.ApplyAspectRatioToVideoRect(source.width, source.height);
        _videoPreviewController.SetRenderTexture(renderTexture);
        _videoPreviewController.FitVideoPreviewToScreen();

        _onVideoPrepared?.Invoke(_videoPreviewController.GetVideoAspectForm());

        StartFadeVolumeTween(0f, _savedVolume);
    }

    private RenderTexture CreateRenderTexture(int width, int height)
    {
        var texture = new RenderTexture(width, height, 0);

        texture.Create();

        return texture;
    }

    public RectTransform GetRectTransform()
    {
        return transform as RectTransform;
    }
}
