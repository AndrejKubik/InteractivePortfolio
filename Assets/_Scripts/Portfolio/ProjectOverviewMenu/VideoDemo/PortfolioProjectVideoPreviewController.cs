using Snek.Utilities;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioProjectVideoPreviewController : SnekMonoBehaviour, ISnekInitializableManual
{
    [SerializeField] private VerticalLayoutGroup _scrollRectContentLayoutGroup;
    [SerializeField] private RectTransform _horizontalLayoutGroupTransform;

    [Space(10f)]
    [SerializeField] private RawImage _videoPreview;

    [Space(10f)]
    [SerializeField] private RectTransform _videoPreviewHeader;
    [SerializeField] private RectTransform _videoPlayerContainerMini;
    [SerializeField] private AspectRatioFitter _aspectRatioFitterMini;
    [SerializeField] private RectTransform _controlsPanelTransform;

    [Space(10f)]
    [SerializeField] private RectTransform _videoPlayerContainerFullscreen;
    [SerializeField] private RectTransform _videoPlayerContainerFullscreenBackground;
    [SerializeField] private AspectRatioFitter _aspectRatioFitterFullscreen;

    private Canvas _canvas;
    private LayoutElement _layoutElement;

    private RectTransform _videoPreviewTransform;

    private const float VideoTopPadding = 15f;

    private float _headerHeight = 0f;
    private float _controlsPanelHeight = 0f;
    private float _videoVerticalPadding = 0f;

    protected override void OnInitialize()
    {
        GetEssentialComponent(out _canvas, SnekGetComponentContext.Parents);
        GetEssentialComponent(out _layoutElement);
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_scrollRectContentLayoutGroup, nameof(_scrollRectContentLayoutGroup));
        ValidateEssentialComponent(_horizontalLayoutGroupTransform, nameof(_horizontalLayoutGroupTransform));
        
        ValidateEssentialComponent(_videoPreview, nameof(_videoPreview));
        
        ValidateEssentialComponent(_videoPreviewHeader, nameof(_videoPreviewHeader));
        ValidateEssentialComponent(_videoPlayerContainerMini, nameof(_videoPlayerContainerMini));
        ValidateEssentialComponent(_aspectRatioFitterMini, nameof(_aspectRatioFitterMini));
        ValidateEssentialComponent(_controlsPanelTransform, nameof(_controlsPanelTransform));
        
        ValidateEssentialComponent(_videoPlayerContainerFullscreen, nameof(_videoPlayerContainerFullscreen));
        ValidateEssentialComponent(_videoPlayerContainerFullscreenBackground, nameof(_videoPlayerContainerFullscreenBackground));
        ValidateEssentialComponent(_aspectRatioFitterFullscreen, nameof(_aspectRatioFitterFullscreen));
    }

    protected override void OnInitializationSuccess()
    {
        _videoPreviewTransform = _videoPreview.transform as RectTransform;

        _headerHeight = _videoPreviewHeader.rect.size.y;
        _controlsPanelHeight = _controlsPanelTransform.rect.size.y;
        _videoVerticalPadding = _scrollRectContentLayoutGroup.padding.bottom;

        _aspectRatioFitterMini.enabled = false;

        SetMiniPlayerTransformAnchors();
    }

    private void SetMiniPlayerTransformAnchors()
    {
        _videoPlayerContainerMini.ResetAnchorOffset();
        _videoPlayerContainerMini.SetAnchorOffset(_headerHeight, AnchorOffsetSide.Top);
        _videoPlayerContainerMini.SetAnchorOffset(_controlsPanelHeight, AnchorOffsetSide.Bottom);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_videoPlayerContainerMini);
    }

    public void SetRenderTexture(RenderTexture texture)
    {
        _videoPreview.texture = texture;
    }

    public void ActivateFullscreenPreview()
    {
        MovePreviewToNewParent(_videoPlayerContainerFullscreen);

        _videoPlayerContainerFullscreenBackground.gameObject.SetActive(true);
    }

    public void ActivateMiniPreview()
    {
        MovePreviewToNewParent(_videoPlayerContainerMini);

        _videoPlayerContainerFullscreenBackground.gameObject.SetActive(false);
    }

    private void MovePreviewToNewParent(RectTransform rectTransform)
    {
        _videoPreviewTransform.SetParent(rectTransform, true);
        _videoPreviewTransform.ResetAnchorOffset();
    }

    public void ApplyAspectRatioToVideoRect(float videoWidth, float videoHeight)
    {
        float aspectRatio = videoWidth / videoHeight;

        _aspectRatioFitterMini.aspectRatio = aspectRatio;
        _aspectRatioFitterFullscreen.aspectRatio = aspectRatio;

        AspectRatioFitter.AspectMode aspectMode = aspectRatio > 1f ?
            AspectRatioFitter.AspectMode.WidthControlsHeight : AspectRatioFitter.AspectMode.HeightControlsWidth;

        _aspectRatioFitterMini.aspectMode = aspectMode;

        _aspectRatioFitterMini.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_aspectRatioFitterMini.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_aspectRatioFitterFullscreen.transform as RectTransform);
    }

    public void FitVideoPreviewToScreen()
    {
        ResetHorizontalLayoutGroupHeight();

        switch (GetVideoAspectForm())
        {
            case VideoAspectForm.Landscape:

                FitVideoPreviewToScreenLandscape();
                break;

            case VideoAspectForm.Portrait:

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

    public VideoAspectForm GetVideoAspectForm()
    {
        return _aspectRatioFitterMini.aspectMode switch
        {
            AspectRatioFitter.AspectMode.WidthControlsHeight => VideoAspectForm.Landscape,
            AspectRatioFitter.AspectMode.HeightControlsWidth => VideoAspectForm.Portrait,
            _ => VideoAspectForm.Unsupported,
        };
    }

    private void FitVideoPreviewToScreenLandscape()
    {
        float targetWidth = _horizontalLayoutGroupTransform.rect.size.x / 2f;
        targetWidth -= 2f * VideoTopPadding;

        _layoutElement.preferredWidth = targetWidth;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);
    }

    private void FitVideoPreviewToScreenPortrait()
    {
        float screenHeight = (float)Screen.height / _canvas.scaleFactor;
        float targetHeight = screenHeight - 2f * _videoVerticalPadding;

        _horizontalLayoutGroupTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);

        _layoutElement.preferredWidth = _videoPlayerContainerMini.rect.size.x;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontalLayoutGroupTransform);
    }
}
