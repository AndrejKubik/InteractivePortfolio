using System.Collections;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.Video;

[UseSnekInspector]
public class PortfolioProjectVideoPlayerController : SnekMonoBehaviour, ISnekInitializableWithData<PortfolioProjectVideoPlayerController.Data>
{
    public delegate void OnVideoPreparedCallback(VideoPlayer source);

    public readonly struct Data
    {
        public readonly string VideoURL;
        public readonly OnVideoPreparedCallback OnVideoPrepared;

        public Data(string videoURL, OnVideoPreparedCallback onVideoPrepared)
        {
            VideoURL = videoURL;
            OnVideoPrepared = onVideoPrepared;
        }
    }

    [SerializeField] private VideoPlayer _videoPlayer;

    private string _videoURL = string.Empty;
    private OnVideoPreparedCallback _onVideoPrepared;

    private float _videoTotalTime = 0f;
    public bool IsVideoSeeking { get; private set; }

    private float? _videoSeekTargetTime = null;
    private bool _isForcingChange = false;

    public void PrepareInitializationData(Data data)
    {
        _videoURL = data.VideoURL;
        _onVideoPrepared = data.OnVideoPrepared;
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_videoPlayer, nameof(_videoPlayer));

        if (string.IsNullOrEmpty(_videoURL))
            FailValidation("Invalid video URL provided.");

        if (_onVideoPrepared == null)
            FailValidation("Video prepared callback not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        if (!_isInitializedOnce)
        {
            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.errorReceived += OnVideoErrorReceived;
            _videoPlayer.seekCompleted += OnVideoSeekCompleted;
        }

        _videoPlayer.url = _videoURL;

        _videoPlayer.Prepare();
    }

    protected override void OnDispose()
    {
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
        _videoPlayer.errorReceived -= OnVideoErrorReceived;
        _videoPlayer.seekCompleted -= OnVideoSeekCompleted;
    }

    public bool IsAudioMuted()
    {
        return _videoPlayer.GetDirectAudioMute(0);
    }

    public void SetAudioMute(bool newState)
    {
        _videoPlayer.SetDirectAudioMute(0, newState);
    }

    public void SetAudioVolume(float newValue)
    {
        _videoPlayer.SetDirectAudioVolume(0, newValue);
    }

    public float GetAudioVolume()
    {
        return _videoPlayer.GetDirectAudioVolume(0);
    }

    public bool IsVideoPaused()
    {
        return _videoPlayer.isPaused;
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

    public void PauseVideo()
    {
        _videoPlayer.Pause();
    }

    public void TrySeekVideo(float newTime, bool forceChange)
    {
        if (forceChange)
            _isForcingChange = true;

        if (IsVideoSeeking)
        {
            _videoSeekTargetTime = newTime;

            return;
        }

        _videoSeekTargetTime = null;

        SeekVideo(newTime);
    }

    private void SeekVideo(float newTime)
    {
        _videoPlayer.time = Mathf.Lerp(0f, _videoTotalTime, newTime);

        IsVideoSeeking = true;
    }

    public float GetVideoProgress()
    {
        return Mathf.InverseLerp(0f, _videoTotalTime, (float)_videoPlayer.time);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        _videoTotalTime = (float)_videoPlayer.length;

        _onVideoPrepared.Invoke(source);
    }

    private void OnVideoSeekCompleted(VideoPlayer source)
    {
        StartCoroutine(StopSeekingSequence());
    }

    private IEnumerator StopSeekingSequence() //smoother feedback when waiting for the end of frame
    {
        yield return new WaitForEndOfFrame();

        if (_isForcingChange && _videoSeekTargetTime != null)
        {
            SeekVideo(_videoSeekTargetTime.Value);
            
            _videoSeekTargetTime = null;
            _isForcingChange = false;
        }
        else
            IsVideoSeeking = false;
    }

    private void OnVideoErrorReceived(VideoPlayer source, string message)
    {
        Debug.LogError(message, source.gameObject);
    }
}
