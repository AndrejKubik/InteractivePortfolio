using System;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[UseSnekInspector]
public class HoverOverlay : SnekMonoBehaviour, ISnekInitializableWithData<HoverOverlay.Data>
{
    public readonly struct Data
    {
        public readonly Action OnShow;

        public Data(Action onShow)
        {
            OnShow = onShow;
        }
    }

    private Image _image;
    private Canvas _canvas;

    [Range(0f, 1f)]
    [SerializeField] private float _maxAlpha = 0.8f;

    [Min(0f)]
    [SerializeField] private float _fadeTime = 0.5f;

    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private Action _onShow;

    private float _currentAlpha = 0f;
    private float _currentFadeProgress = 0f;

    public void PrepareInitializationData(Data data)
    {
        _onShow = data.OnShow;
    }

    protected override void OnInitialize()
    {
        GetEssentialComponent(out _image);
        GetEssentialComponent(out _canvas, SnekGetComponentContext.Parents);
    }

    public void HandleMouseHoverUserActivityBased(bool isFadeAllowed)
    {
        if (IsHovered() && IsMouseMoved())
            Show();
        else if (IsOverlayVisible() && isFadeAllowed)
            FadeAlpha();
    }

    public void HandleMouseHoverConstant()
    {
        if (IsHovered())
            Show();
        else if (IsOverlayVisible())
            FadeAlpha();
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

        _onShow?.Invoke();
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
