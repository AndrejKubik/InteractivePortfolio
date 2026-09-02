using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class VideoPlayerVolumeSliderAnimator : SnekMonoBehaviour
{
    private Canvas _canvas;

    [SerializeField] private RectTransform _muteButtonTransform;
    [SerializeField] private RectTransform _volumeSliderContainerTransform;

    [Min(0f)]
    [SerializeField] private float _maxSliderHeight = 150f;

    [Tooltip("Duration for reaching max height in seconds.")]
    [Min(0f)]
    [SerializeField] private float _animationDuration = 0.25f;
    [SerializeField] private AnimationCurve _animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private float _unhoverGraceTime = 0.25f;

    private bool _isExpanding = false;
    private float _currentAnimationTime = 0f;
    private float _currentUnhoverGraceTime = 0f;

    protected override void Initialize()
    {
        GetEssentialComponent(out _canvas, SnekGetComponentContext.Parents);
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_muteButtonTransform, nameof(_muteButtonTransform));
        ValidateEssentialComponent(_volumeSliderContainerTransform, nameof(_volumeSliderContainerTransform));

        if (_animationCurve == null || _animationCurve.keys.Length < 1)
            FailValidation("Animation curve does not exist or has no keys assigned.");
    }

    private void Update()
    {
        if (IsAnyRectHovered())
            _isExpanding = true;

        if (_isExpanding)
        {
            _currentAnimationTime += Time.deltaTime;
            _currentAnimationTime = Mathf.Min(_currentAnimationTime, _animationDuration);

            _currentUnhoverGraceTime = 0f;

            UpdateVolumeSliderContainerHeight();

            if (_currentAnimationTime >= _animationDuration)
                _isExpanding = false;
        }
        else
        {
            if (_currentUnhoverGraceTime < _unhoverGraceTime)
            {
                _currentUnhoverGraceTime += Time.deltaTime;
                _currentUnhoverGraceTime = Mathf.Min(_currentUnhoverGraceTime, _unhoverGraceTime);
            }
            else if (_currentAnimationTime > 0f)
            {
                _currentAnimationTime -= Time.deltaTime;
                _currentAnimationTime = Mathf.Max(0f, _currentAnimationTime);

                UpdateVolumeSliderContainerHeight();
            }
        }
    }

    private bool IsAnyRectHovered()
    {
        return _muteButtonTransform.IsHovered(_canvas) || _volumeSliderContainerTransform.IsHovered(_canvas);
    }

    private float GetAnimationProgress()
    {
        return _currentAnimationTime / _animationDuration;
    }

    private float GetCurvedAnimationProgress()
    {
        return _animationCurve.Evaluate(GetAnimationProgress());
    }

    private void UpdateVolumeSliderContainerHeight()
    {
        float targetHeight = Mathf.LerpUnclamped(0f, _maxSliderHeight, GetCurvedAnimationProgress());

        _volumeSliderContainerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
    }
}
