using System;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectVideoControlsPanelController : SnekMonoBehaviour, ISnekInitializableWithData<PortfolioProjectVideoControlsPanelController.Data>
{
    public readonly struct Data
    {
        public readonly Action<float> OnMoveTimeline;
        public readonly Action<float> OnChangeAudioVolume;
        public readonly Action OnMuteAudioButtonClick;
        public readonly Action OnToggleFullscreenButtonClick;
        public readonly Action OnPlayPauseButtonClick;

        public Data(
            Action<float> onMoveTimeline,
            Action<float> onChangeAudioVolume,
            Action onMuteAudioButtonClick,
            Action onToggleFullscreenButtonClick,
            Action onPlayPauseButtonClick)
        {
            OnMoveTimeline = onMoveTimeline;
            OnChangeAudioVolume = onChangeAudioVolume;
            OnMuteAudioButtonClick = onMuteAudioButtonClick;
            OnToggleFullscreenButtonClick = onToggleFullscreenButtonClick;
            OnPlayPauseButtonClick = onPlayPauseButtonClick;
        }
    }

    [SerializeField] private RectTransform _controlsPanel;

    [Space(10f)]
    [SerializeField] private PortfolioProjectVideoDemoTimeline _videoTimeline;
    [SerializeField] private VideoPlayerVolumeSlider _volumeSlider;
    [SerializeField] private AudioMuteButton _volumeMuteButton;
    [SerializeField] private PortfolioProjectVideoDemoOverlayButton _playPauseControlButton;
    [SerializeField] private VideoPlayerToggleFullScreenButton _toggleFullScreenButton;

    [Min(0f)]
    [SerializeField] private float _fullscreenHoverControlsPanelShowTimeMax = 3f;

    [SerializeField] private AnimationCurve _fullscreenControlsPanelHideCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private Action<float> _onMoveTimeline;
    private Action<float> _onChangeAudioVolume;
    private Action _onMuteAudioButtonClick;
    private Action _onToggleFullscreenButtonClick;
    private Action _onPlayPauseButtonClick;

    private float _controlsPanelHeight = 0f;
    private float _fullscreenHoverControlsPanelShowTime = 0f;

    public void PrepareInitializationData(Data data)
    {
        _onMoveTimeline = data.OnMoveTimeline;
        _onChangeAudioVolume = data.OnChangeAudioVolume;
        _onMuteAudioButtonClick = data.OnMuteAudioButtonClick;
        _onToggleFullscreenButtonClick = data.OnToggleFullscreenButtonClick;
        _onPlayPauseButtonClick = data.OnPlayPauseButtonClick;
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_controlsPanel, nameof(_controlsPanel));

        ValidateEssentialComponent(_videoTimeline, nameof(_videoTimeline));
        ValidateEssentialComponent(_volumeSlider, nameof(_volumeSlider));
        ValidateEssentialComponent(_volumeMuteButton, nameof(_volumeMuteButton));
        ValidateEssentialComponent(_playPauseControlButton, nameof(_playPauseControlButton));
        ValidateEssentialComponent(_toggleFullScreenButton, nameof(_toggleFullScreenButton));

        if (_onMoveTimeline == null)
            FailValidation("User move timeline callback not assigned.");

        if (_onChangeAudioVolume == null)
            FailValidation("User change audio volume callback not assigned.");

        if (_onMuteAudioButtonClick == null)
            FailValidation("User mute audio callback not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _controlsPanelHeight = _controlsPanel.rect.height;

        _videoTimeline.Initialize(new PortfolioProjectVideoDemoTimeline.Data(_onMoveTimeline));
        _volumeSlider.Initialize(new VideoPlayerVolumeSlider.Data(OnVolumeChange));

        _volumeMuteButton.SetExternalCallback(_onMuteAudioButtonClick);
        _playPauseControlButton.SetExternalCallback(_onPlayPauseButtonClick);
        _toggleFullScreenButton.SetExternalCallback(_onToggleFullscreenButtonClick);

        _videoTimeline.SetValue(0f);
    }

    private void OnVolumeChange(float newValue)
    {
        PlayerPrefs.SetFloat(SaveKeys.VideoDemoVolume, newValue);

        _onChangeAudioVolume.Invoke(newValue);
    }

    public void ControllFullscreenControlsPanelAnimation()
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

    public void ShowFullscreenControlPanel()
    {
        MoveControlsPanelToFullscreenPlayerDefaultPosition();

        _fullscreenHoverControlsPanelShowTime = 0f;
    }

    public void MoveControlsPanelToMiniPlayerDefaultPosition()
    {
        _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, 0f);
    }

    private void MoveControlsPanelToFullscreenPlayerDefaultPosition()
    {
        _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, _controlsPanelHeight);
    }

    public bool IsFullscreenControlsPanelVisible()
    {
        return _fullscreenHoverControlsPanelShowTime < _fullscreenHoverControlsPanelShowTimeMax;
    }

    public void SetTimelineProgress(float newValue)
    {
        _videoTimeline.SetValue(newValue);
    }

    public bool IsTimelineSliderHandleHeld()
    {
        return _videoTimeline.IsHandleHeld;
    }

    public void SetAudioVolume(float newValue)
    {
        _volumeSlider.SetValue(newValue, false);
    }

    public float GetAudioVolume()
    {
        return _volumeSlider.GetValue();
    }

    public void SetPlayPauseButtonSymbol(Sprite newSymbol)
    {
        _playPauseControlButton.SetSymbol(newSymbol);
    }

    public void MatchMuteAudioButtonSymbolWithVolume()
    {
        _volumeMuteButton.MatchSpriteWithVolume(GetAudioVolume());
    }

    public void SetToggleFullscreenButtonSymbol(Sprite newSymbol)
    {
        _toggleFullScreenButton.SetSymbol(newSymbol);
    }

    public void SetMuteAudioButtonMutedSymbol()
    {
        _volumeMuteButton.SetMutedSymbol();
    }
}
