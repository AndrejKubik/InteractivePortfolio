using System;
using DG.Tweening;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(PortfolioProjectVideoPlayerControllerMono))]
[RequireComponent(typeof(PortfolioProjectVideoPreviewController))]
[RequireComponent(typeof(PortfolioProjectVideoHoverOverlayController))]
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
    [SerializeField] private RectTransform _controlsPanel;
    [SerializeField] private PortfolioProjectVideoDemoTimeline _videoTimeline;
    [SerializeField] private VideoPlayerVolumeSlider _volumeSlider;

    [Space(10f)]
    [SerializeField] private AudioMuteButton _volumeMuteButton;
    [SerializeField] private Sprite _mutedSymbol;
    [SerializeField] private Sprite _unmutedSymbol;

    [Space(10f)]
    [SerializeField] private PortfolioProjectVideoDemoOverlayButton _playPauseControlButton;
    [SerializeField] private Sprite _playSymbol;
    [SerializeField] private Sprite _pauseSymbol;

    [Min(0f)]
    [SerializeField] private float _fullscreenHoverControlsPanelShowTimeMax = 3f;

    [SerializeField] private AnimationCurve _fullscreenControlsPanelHideCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Space(10f)]
    [SerializeField] private VideoPlayerToggleFullScreenButton _toggleFullScreenButton;
    [SerializeField] private Sprite _fullscreenOnSymbol;
    [SerializeField] private Sprite _fullscreenOffSymbol;

    private string _videoURL = string.Empty;
    private float _slideDuration = 0f;
    private Action<VideoAspectForm> _onVideoPrepared = null;

    private PortfolioProjectVideoPlayerControllerMono _videoPlayerController;
    private PortfolioProjectVideoPreviewController _videoPreviewController;
    private PortfolioProjectVideoHoverOverlayController _videoHoverOverlayController;


    private float _controlsPanelHeight = 0f;

    private Tween _activeVolumeTween = null;

    private float _savedVolume = 0f;
    private bool _savedMuteState = false;

    private bool _isFullscreen = false;
    private float _fullscreenHoverControlsPanelShowTime = 0f;

    private bool _isPlayPauseButtonClicked = false;

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
    }

    protected override void Validate()
    {
        if (string.IsNullOrEmpty(_videoURL))
            FailValidation("Invalid video URL provided.");
        
        ValidateEssentialComponent(_videoPlayer, nameof(_videoPlayer));
        ValidateEssentialComponent(_controlsPanel, nameof(_controlsPanel));
        ValidateEssentialComponent(_videoTimeline, nameof(_videoTimeline));
        ValidateEssentialComponent(_volumeSlider, nameof(_volumeSlider));
        ValidateEssentialComponent(_volumeMuteButton, nameof(_volumeMuteButton));
        ValidateEssentialComponent(_mutedSymbol, nameof(_mutedSymbol));
        ValidateEssentialComponent(_unmutedSymbol, nameof(_unmutedSymbol));
        ValidateEssentialComponent(_playPauseControlButton, nameof(_playPauseControlButton));
        ValidateEssentialComponent(_playSymbol, nameof(_playSymbol));
        ValidateEssentialComponent(_pauseSymbol, nameof(_pauseSymbol));

        ValidateEssentialComponent(_toggleFullScreenButton, nameof(_toggleFullScreenButton));
        
        ValidateEssentialComponent(_fullscreenOnSymbol, nameof(_fullscreenOnSymbol));
        ValidateEssentialComponent(_fullscreenOffSymbol, nameof(_fullscreenOffSymbol));
    }

    protected override void OnInitializationSuccess()
    {
        _videoPlayerController.Initialize(new PortfolioProjectVideoPlayerControllerMono.Data(
            _videoURL,
            OnVideoPrepared));

        _videoPreviewController.Initialize();

        _videoHoverOverlayController.Initialize(new PortfolioProjectVideoHoverOverlayController.Data(
            OnPlayPauseButtonClick,
            OnShowHoverOverlay));

        _videoTimeline.Initialize(new PortfolioProjectVideoDemoTimeline.Data(OnUserMoveTimeline));
        _volumeSlider.Initialize(new VideoPlayerVolumeSlider.Data(OnVolumeChange));

        _volumeMuteButton.SetExternalCallback(OnMuteButtonClick);

        _playPauseControlButton.SetExternalCallback(OnPlayPauseButtonClick);
        _playPauseControlButton.SetSymbol(_pauseSymbol);

        _toggleFullScreenButton.SetExternalCallback(OnToggleFullscreenButtonClick);

        _isFullscreen = false;
        _controlsPanelHeight = _controlsPanel.rect.height;

        LoadAudioSettings();
    }


    private void Update()
    {
        if (!_videoPlayerController.IsVideoSeeking && !_videoTimeline.IsHandleHeld)
            UpdateTimelineSlider();

        _videoHoverOverlayController.HandleMouseHover(_isFullscreen, IsHoverOverlayFadeAllowed());

        if (_isPlayPauseButtonClicked)
            _isPlayPauseButtonClicked = false;

        if (_isFullscreen)
            HandleFullscreenControlsPanelAnimation();
        else
            _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, 0f);
    }

    private bool IsHoverOverlayFadeAllowed()
    {
        return !IsControlsPanelRequired() && !_isPlayPauseButtonClicked;
    }

    private void HandleFullscreenControlsPanelAnimation()
    {
        float showProgress = _fullscreenHoverControlsPanelShowTime / _fullscreenHoverControlsPanelShowTimeMax;
        float showProgressCurved = _fullscreenControlsPanelHideCurve.Evaluate(showProgress);

        float hidePositionY = 0f;
        float showPositionY = _controlsPanelHeight;

        float targetPositionY = Mathf.Lerp(hidePositionY, showPositionY, showProgressCurved);

        _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, targetPositionY);

        _fullscreenHoverControlsPanelShowTime += Time.deltaTime;

        _fullscreenHoverControlsPanelShowTime = Mathf.Min(
            _fullscreenHoverControlsPanelShowTime,
            _fullscreenHoverControlsPanelShowTimeMax);
    }

    private bool IsControlsPanelRequired()
    {
        return _fullscreenHoverControlsPanelShowTime < _fullscreenHoverControlsPanelShowTimeMax;
    }

    private void OnShowHoverOverlay()
    {
        if (!_isFullscreen)
            return;

        float targetControlsPanelPositionY = _controlsPanelHeight;

        _controlsPanel.anchoredPosition = new Vector2(
            _controlsPanel.anchoredPosition.x,
            targetControlsPanelPositionY);

        ShowFullscreenControlPanel();
    }

    private void UpdateTimelineSlider()
    {
        _videoTimeline.SetValue(_videoPlayerController.GetVideoProgress(), false);
    }

    private void LoadAudioSettings()
    {
        _savedVolume = PlayerPrefs.GetFloat(SaveKeys.VideoDemoVolume, 1f);
        _savedMuteState = Convert.ToBoolean(PlayerPrefs.GetInt(SaveKeys.VideoDemoMute, 0));

        _volumeSlider.SetValue(_savedVolume, false);

        SetAudioMute(_savedMuteState);
    }

    public void FadeVolume()
    {
        StartFadeVolumeTween(_volumeSlider.Slider.value, 0f);
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
            _playPauseControlButton.SetSymbol(_pauseSymbol);

            _videoHoverOverlayController.ShowFadingOverlay(_playSymbol);
        }
        else
        {
            _videoPlayerController.PauseVideo();
            _playPauseControlButton.SetSymbol(_pauseSymbol);

            _videoHoverOverlayController.ShowFadingOverlay(_pauseSymbol);
        }

        _isPlayPauseButtonClicked = true;

        if (_isFullscreen)
            ShowFullscreenControlPanel();
    }

    private void ShowFullscreenControlPanel()
    {
        _fullscreenHoverControlsPanelShowTime = 0f;
    }

    private void OnUserMoveTimeline(float newTime)
    {
        _videoPlayerController.SeekVideo(newTime);
    }

    private void OnVolumeChange(float newValue)
    {
        SetAudioVolume(newValue);

        PlayerPrefs.SetFloat(SaveKeys.VideoDemoVolume, newValue);
    }

    private void SetAudioVolume(float newValue)
    {
        _videoPlayerController.SetAudioVolume(newValue);
        _volumeMuteButton.MatchSpriteWithVolume(newValue);

        SetAudioMute(false);
    }

    private void OnMuteButtonClick()
    {
        bool newState = !_videoPlayerController.IsAudioMuted();

        SetAudioMute(newState);

        if (newState == true)
            _videoHoverOverlayController.ShowFadingOverlay(_mutedSymbol);
        else
            _videoHoverOverlayController.ShowFadingOverlay(_unmutedSymbol);
    }

    private void OnToggleFullscreenButtonClick()
    {
        _isFullscreen = !_isFullscreen;

        _videoPreviewController.SetFullscreenMode(_isFullscreen);

        float targetControlsPanelPositionY = _isFullscreen ?
            _controlsPanelHeight : 0f;

        _controlsPanel.anchoredPosition = new Vector2(
            _controlsPanel.anchoredPosition.x,
            targetControlsPanelPositionY);

        Sprite fullscreenButtonSymbol = _isFullscreen ?
            _fullscreenOffSymbol : _fullscreenOnSymbol;

        _toggleFullScreenButton.SetSymbol(fullscreenButtonSymbol);
    }

    private void SetAudioMute(bool newState)
    {
        _videoPlayerController.SetAudioMute(newState);

        if (newState == true)
            _volumeMuteButton.SetMutedSymbol();
        else
            _volumeMuteButton.MatchSpriteWithVolume(_videoPlayerController.GetAudioVolume());

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
