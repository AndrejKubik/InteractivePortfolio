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
    private string _videoURL = string.Empty;

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _videoPreview;
    [SerializeField] private RectTransform _videoPreviewHeader;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;

    private RenderTexture _renderTexture;
    private Action _onVideoPrepared;

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
        _videoPlayer.url = _videoURL;
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        _videoPlayer.Prepare();
        
    }

    private void OnDestroy()
    {
        if (_isValid)
            _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        CreateAndApplyRenderTexture(source);
        ApplyAspectRatioToVideoRectSize(source);
        FitVideoPreviewToScreen();

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

    private void FitVideoPreviewToScreenLandscape()
    {
        var horizontalLayoutGroupTransform = _horizontalLayoutGroup.transform as RectTransform;

        float targetWidth = horizontalLayoutGroupTransform.rect.size.x / 2f;
        targetWidth -= 2f * VideoVerticalPadding;

        _layoutElement.preferredWidth = targetWidth;

        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroupTransform);
    }

    private void FitVideoPreviewToScreenPortrait()
    {
        var horizontalLayoutGroupTransform = _horizontalLayoutGroup.transform as RectTransform;

        float targetHeight = (float)Screen.height - 2f * VideoVerticalPadding;
        targetHeight -= _videoPreviewHeader.rect.size.y;

        horizontalLayoutGroupTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroupTransform);

        var videoPlayerRectTransform = _videoPlayer.transform as RectTransform;

        _layoutElement.preferredWidth = videoPlayerRectTransform.rect.size.x;

        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroupTransform);
    }

    public RectTransform GetRectTransform()
    {
        return transform as RectTransform;
    }
}
