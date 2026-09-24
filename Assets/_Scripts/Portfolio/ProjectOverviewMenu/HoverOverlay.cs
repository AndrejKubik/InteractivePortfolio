using System;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[UseSnekInspector]
public class HoverOverlay : SnekMonoBehaviour
{
    private Image _image;
    private Canvas _canvas;

    [Range(0f, 1f)]
    [SerializeField] private float _maxAlpha = 0.8f;

    [Min(0f)]
    [SerializeField] private float _fadeTime = 0.5f;

    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private float _currentAlpha = 0f;
    private float _currentFadeProgress = 0f;

    protected override void Initialize()
    {
        GetEssentialComponent(out _image);
        GetEssentialComponent(out _canvas, SnekGetComponentContext.Parents);
    }

    public void HandleMouseHover(bool isFullscreen, bool isFullAlphaLocked, Action onShowOverlay)
    {
        if (isFullscreen)
        {
            if (IsHovered() && IsMouseMoved())
            {
                Show();

                onShowOverlay.Invoke();
            }
            else if (IsOverlayVisible() && !isFullAlphaLocked)
                FadeAlpha();
        }
        else
        {
            if (IsHovered())
            {
                Show();

                onShowOverlay.Invoke();
            }
            else if (IsOverlayVisible())
                FadeAlpha();
        }
    }

    private bool IsHovered()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            _image.rectTransform,
            Input.mousePosition,
            _canvas.worldCamera);
    }

    private bool IsMouseMoved()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        return mouseDelta.sqrMagnitude > 0f;
    }

    private bool IsOverlayVisible()
    {
        return _currentFadeProgress > 0f;
    }

    public void Show()
    {
        _currentAlpha = _maxAlpha;

        _currentFadeProgress = 1f;

        UpdateImageAlpha();
    }

    private void FadeAlpha()
    {
        _currentFadeProgress = Mathf.MoveTowards(
            _currentFadeProgress,
            0f,
            Time.deltaTime / _fadeTime);

        UpdateImageAlpha();
    }

    private void UpdateImageAlpha()
    {
        float curvedProgress = _fadeCurve.Evaluate(_currentFadeProgress);

        _currentAlpha = Mathf.LerpUnclamped(_maxAlpha, 0f, curvedProgress);

        _image.color = _image.color.ChangeAlpha(_currentAlpha);
    }
}
