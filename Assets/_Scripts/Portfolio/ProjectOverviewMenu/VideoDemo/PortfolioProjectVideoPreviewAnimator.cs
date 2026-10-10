using DG.Tweening;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectVideoPreviewAnimator : SnekMonoBehaviour, ISnekInitializableWithData<PortfolioProjectVideoPreviewAnimator.Data>
{
    public readonly struct Data
    {
        public readonly RectTransform VideoPreview;
        public readonly RectTransform VideoBackground;
        public readonly RectTransform FullscreenPlayerContainer;
        public readonly RectTransform MiniPlayerContainer;

        public Data(
            RectTransform videoPreview,
            RectTransform videoBackground,
            RectTransform fullscreenPlayerContainer,
            RectTransform miniPlayerContainer)
        {
            VideoPreview = videoPreview;
            VideoBackground = videoBackground;
            FullscreenPlayerContainer = fullscreenPlayerContainer;
            MiniPlayerContainer = miniPlayerContainer;
        }
    }


    [Header("Enter Fullscreen Animation")]
    [Min(0f)]
    [SerializeField] private float _movePreviewInDuration = 0.25f;
    [SerializeField] private AnimationCurve _movePreviewInPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve _movePreviewInScaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Min(0f)]
    [SerializeField] private float _spreadPreviewDuration = 0.25f;
    [SerializeField] private AnimationCurve _spreadPreviewCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Min(0f)]
    [SerializeField] private float _spreadBackgroundDuration = 0.25f;
    [SerializeField] private AnimationCurve _spreadBackgroundCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    [Header("Exit Fullscreen Animation")]
    [Min(0f)]
    [SerializeField] private float _movePreviewOutDuration = 0.25f;
    [SerializeField] private AnimationCurve _movePreviewOutPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve _movePreviewOutScaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Min(0f)]
    [SerializeField] private float _shrinkBackgroundDuration = 0.25f;
    [SerializeField] private AnimationCurve _shrinkBackgroundCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Min(0f)]
    [SerializeField] private float _shrinkPreviewDuration = 0.25f;
    [SerializeField] private AnimationCurve _shrinkPreviewCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    private RectTransform _videoPreview;
    private RectTransform _videoBackground;
    private RectTransform _fullscreenPlayerContainer;
    private RectTransform _miniPlayerContainer;

    public void PrepareInitializationData(Data data)
    {
        _videoPreview = data.VideoPreview;
        _videoBackground = data.VideoBackground;
        _fullscreenPlayerContainer = data.FullscreenPlayerContainer;
        _miniPlayerContainer = data.MiniPlayerContainer;
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_videoPreview, nameof(_videoPreview));
        ValidateEssentialComponent(_videoBackground, nameof(_videoBackground));
        ValidateEssentialComponent(_fullscreenPlayerContainer, nameof(_fullscreenPlayerContainer));
        ValidateEssentialComponent(_miniPlayerContainer, nameof(_miniPlayerContainer));
    }

    public void PlayFullscreenEnterAnimation()
    {
        _videoPreview.DOKill();
        _videoBackground.DOKill();

        MovePreviewToNewParent(_fullscreenPlayerContainer);
        MovePreviewIn();
    }

    private void MovePreviewIn()
    {
        DOTween.Sequence(_videoPreview)
            .Append(_videoPreview.DOSnekAnchoredPosition(Vector2.zero, _movePreviewInDuration, _movePreviewInPositionCurve))
            .Join(_videoPreview.DOSnekScale(Vector3.one * 0.75f, _movePreviewInDuration, _movePreviewInScaleCurve))
            .OnComplete(SpreadPreview);
    }

    private void SpreadPreview()
    {
        DOTween.Sequence(_videoPreview)
            .Append(_videoPreview.DOSnekAnchorOffsetMin(Vector2.zero, _spreadPreviewDuration, _spreadPreviewCurve))
            .Join(_videoPreview.DOSnekAnchorOffsetMax(Vector2.zero, _spreadPreviewDuration, _spreadPreviewCurve))
            .Join(_videoPreview.DOSnekScale(Vector3.one, _movePreviewInDuration, _spreadPreviewCurve))
            .OnComplete(SpreadBackground);
    }

    private void SpreadBackground()
    {
        DOTween.Sequence(_videoBackground)
            .Append(_videoBackground.DoSnekAnchorMin(Vector2.zero, _spreadBackgroundDuration, _spreadBackgroundCurve))
            .Join(_videoBackground.DoSnekAnchorMax(Vector2.one, _spreadBackgroundDuration, _spreadBackgroundCurve));
    }

    public void PlayFullscreenExitAnimation()
    {
        _videoPreview.DOKill();
        _videoBackground.DOKill();

        ShrinkBackground();
    }

    private void ShrinkBackground()
    {
        float previewWidth = _videoPreview.rect.width;
        float backgroundWidth = _videoBackground.rect.width;

        bool isBackgroundVisible = previewWidth < backgroundWidth && !Mathf.Approximately(previewWidth, backgroundWidth);

        float tweenDuration = isBackgroundVisible ? _shrinkBackgroundDuration : 0f;

        DOTween.Sequence(_videoBackground)
            .Append(_videoBackground.DoSnekAnchorMin(new Vector2(0.5f, 0f), tweenDuration, _shrinkBackgroundCurve))
            .Join(_videoBackground.DoSnekAnchorMax(new Vector2(0.5f, 1f), tweenDuration, _shrinkBackgroundCurve))
            .OnComplete(OnFinishShrinkBackground);
    }

    private void OnFinishShrinkBackground()
    {
        MovePreviewToNewParent(_miniPlayerContainer);
        MovePreviewOut();
    }

    private void MovePreviewOut()
    {
        DOTween.Sequence(_videoPreview)
            .Append(_videoPreview.DOSnekAnchoredPosition(Vector2.zero, _movePreviewOutDuration, _movePreviewOutPositionCurve))
            .Join(_videoPreview.DOSnekScale(Vector3.one * 0.35f, _movePreviewOutDuration, _movePreviewOutScaleCurve))
            .OnComplete(ShrinkPreview);
    }

    private void ShrinkPreview()
    {
        DOTween.Sequence(_videoPreview)
            .Append(_videoPreview.DOSnekAnchorOffsetMin(Vector2.zero, _shrinkPreviewDuration, _shrinkPreviewCurve))
            .Join(_videoPreview.DOSnekAnchorOffsetMax(Vector2.zero, _shrinkPreviewDuration, _shrinkPreviewCurve))
            .Join(_videoPreview.DOSnekScale(Vector3.one, _movePreviewOutDuration, _shrinkPreviewCurve));
    }

    private void MovePreviewToNewParent(RectTransform rectTransform)
    {
        _videoPreview.SetParent(rectTransform, true);
    }
}
