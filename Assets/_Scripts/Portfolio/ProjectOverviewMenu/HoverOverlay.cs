using Snek.Utilities;
using UnityEngine;
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

    private float _currentAlpha = 0f;

    protected override void Initialize()
    {
        GetEssentialComponent(out _image);
        GetEssentialComponent(out _canvas, SnekGetComponentContext.Parents);
    }

    private void Update()
    {
        if (IsHovered())
            Show();
        else if (_currentAlpha > 0f)
            FadeAlpha();
    }

    private bool IsHovered()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            _image.rectTransform,
            Input.mousePosition,
            _canvas.worldCamera);
    }

    private void Show()
    {
        _currentAlpha = 0.8f;

        UpdateImageAlpha();
    }

    private void FadeAlpha()
    {
        _currentAlpha = Mathf.MoveTowards(
            _currentAlpha,
            0f,
            Time.deltaTime / _fadeTime);

        UpdateImageAlpha();
    }

    private void UpdateImageAlpha()
    {
        _image.color = _image.color.ChangeAlpha(_currentAlpha);
    }
}
