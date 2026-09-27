using System;
using System.Collections;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PortfolioProjectVideoPlayerController : SnekMonoSubcomponent, ISnekInitializableWithData<PortfolioProjectVideoPlayerController.Data>
{
    public delegate void OnVideoPreparedCallback(VideoPlayer source);

    public readonly struct Data
    {
        public readonly VideoPlayer VideoPlayer;
        public readonly string VideoURL;
        public readonly OnVideoPreparedCallback OnVideoPrepared;

        public Data(VideoPlayer videoPlayer, string videoURL, OnVideoPreparedCallback onVideoPrepared)
        {
            VideoPlayer = videoPlayer;
            VideoURL = videoURL;
            OnVideoPrepared = onVideoPrepared;
        }
    }

    private VideoPlayer _videoPlayer;
    private string _videoURL = string.Empty;
    private OnVideoPreparedCallback _onVideoPrepared;

    private float _videoTotalTime = 0f;
    public bool IsVideoSeeking { get; private set; }

    public void PrepareInitializationData(Data data)
    {
        _videoPlayer = data.VideoPlayer;
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
        if (!_isValid)
            return;

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

    public void SeekVideo(float newTime)
    {
        if (IsVideoSeeking)
            return;

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

        PlayVideo();
    }

    private void OnVideoSeekCompleted(VideoPlayer source)
    {
        _parentComponent.StartCoroutine(StopSeekingSequence());
    }

    private IEnumerator StopSeekingSequence() //smoother feedback when waiting for the end of frame
    {
        yield return new WaitForEndOfFrame();

        IsVideoSeeking = false;
    }

    private void OnVideoErrorReceived(VideoPlayer source, string message)
    {
        Debug.LogError(message, source.gameObject);
    }
}
