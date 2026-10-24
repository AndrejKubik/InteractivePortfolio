using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[UseSnekInspector]
public class PortfolioProjectButtonTag : SnekMonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _textMesh;

    private RectTransform _rectTransform;

    private float _paddingLeft = 0f;
    private float _paddingRight = 0f;

    protected override void OnInitialize()
    {
        GetEssentialComponent(out _rectTransform);
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_background, nameof(_background));
        ValidateEssentialComponent(_textMesh, nameof(_textMesh));
    }

    protected override void OnInitializationSuccess()
    {
        RectTransform textMeshTransform = _textMesh.rectTransform;

        _paddingLeft = textMeshTransform.offsetMin.x;
        _paddingRight = -textMeshTransform.offsetMax.x;
    }

    public void SetColor(Color newColor)
    {
        _background.color = newColor;
    }

    public void SetText(string text)
    {
        _textMesh.SetText(text);

        AdjustWidthToFitText();
    }

    private void AdjustWidthToFitText()
    {
        float requiredWidth = _textMesh.GetPreferredValues(_textMesh.text, Mathf.Infinity, Mathf.Infinity).x;
        requiredWidth += _paddingLeft + _paddingRight;

        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, requiredWidth);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }
}
