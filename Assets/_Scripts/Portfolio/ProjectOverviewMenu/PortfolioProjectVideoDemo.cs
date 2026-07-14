using System;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[UseSnekInspector]
public class PortfolioProjectVideoDemo : SnekMonoBehaviour
{
    private RectTransform _rectTransform;

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _videoPreview;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;

    private RenderTexture _renderTexture;

    protected override void Initialize()
    {
        _rectTransform = transform as RectTransform;
    }

    protected override void Validate()
    {
        if (!_rectTransform)
            FailValidation("Cannot find Rect Transform component.");

        if (!_videoPlayer)
            FailValidation("Video player not assigned.");

        if (!_videoPreview)
            FailValidation("Video preview not assigned.");

        if (!_aspectRatioFitter)
            FailValidation("Aspect ratio fitter not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnDestroy()
    {
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    public void ApplyData(string videoURL)
    {
        if (string.IsNullOrEmpty(videoURL))
        {
            Debug.LogError("Invalid video URL provided, cannot apply data.");

            return;
        }

        _videoPlayer.url = videoURL;

        _videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        _aspectRatioFitter.aspectRatio = (float)source.width / (float)source.height;
        _aspectRatioFitter.enabled = true;

        _renderTexture = new RenderTexture((int)source.width, (int)source.height, 0);

        _renderTexture.Create();

        _videoPlayer.targetTexture = _renderTexture;
        _videoPreview.texture = _renderTexture;

        var videoPlayerRectTransform = _videoPlayer.transform as RectTransform;

        GetComponent<LayoutElement>().minWidth = videoPlayerRectTransform.rect.size.x;

        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, videoPlayerRectTransform.rect.size.x);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

        _videoPlayer.Play();
    }
}
