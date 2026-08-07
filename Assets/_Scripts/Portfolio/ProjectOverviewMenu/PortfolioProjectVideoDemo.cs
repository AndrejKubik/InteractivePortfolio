using System;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[UseSnekInspector]
public class PortfolioProjectVideoDemo : SnekMonoBehaviour
{
    private const float VideoVerticalPadding = 15f;

    private LayoutElement _layoutElement;

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _videoPreview;
    [SerializeField] private RectTransform _videoPreviewHeader;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;

    private RenderTexture _renderTexture;
    private Action _onVideoPreparedCallback;

    protected override void Initialize()
    {
        GetEssentialComponent(out _layoutElement);
    }

    protected override void Validate()
    {
        if (!_videoPlayer)
            FailValidation("Video player not assigned.");

        if (!_videoPreview)
            FailValidation("Video preview not assigned.");

        if (!_videoPreviewHeader)
            FailValidation("Video preview header not assigned.");

        if (!_aspectRatioFitter)
            FailValidation("Aspect ratio fitter not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        _videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnDestroy()
    {
        if (_isValid)
            _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    public void Initialize(string videoURL, Action onVideoPreparedCallback)
    {
        if (string.IsNullOrEmpty(videoURL))
        {
            Debug.LogError("Invalid video URL provided, cannot apply data.");

            return;
        }

        _videoPlayer.url = videoURL;
        _onVideoPreparedCallback = onVideoPreparedCallback;

        _videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        CreateAndApplyRenderTexture(source);
        ApplyAspectRatioToVideoRectSize(source);
        FitVideoPreviewToScreen();

        _videoPlayer.Play();

        _onVideoPreparedCallback.Invoke();
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
        _aspectRatioFitter.aspectRatio = (float)source.width / (float)source.height;
        _aspectRatioFitter.enabled = true;
    }

    private void FitVideoPreviewToScreen()
    {
        var horizontalLayoutGroupTransform = _horizontalLayoutGroup.transform as RectTransform;
        float targetHeight = (float)Screen.height - 2f * VideoVerticalPadding - _videoPreviewHeader.rect.size.y;

        horizontalLayoutGroupTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroupTransform);

        var videoPlayerRectTransform = _videoPlayer.transform as RectTransform;

        _layoutElement.preferredWidth = videoPlayerRectTransform.rect.size.x;

        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroupTransform);
    }
}
