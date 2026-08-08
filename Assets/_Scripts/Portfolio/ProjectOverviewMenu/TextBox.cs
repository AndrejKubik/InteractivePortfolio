using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class TextBox : SnekMonoBehaviour
{
    private RectTransform _rectTransform;
    
    [SerializeField] private RectTransform _header;
    [SerializeField] private TextMeshProUGUI _textMesh;

    protected override void Initialize()
    {
        GetEssentialComponent(out _rectTransform);
    }

    protected override void Validate()
    {
        if (!_header)
            FailValidation("Header not assigned.");

        if (!_textMesh)
            FailValidation("Text mesh not assigned.");
    }

    public void SetText(string text)
    {
        _textMesh.SetText(text);
        _textMesh.ForceMeshUpdate();

        float targetHeight = _textMesh.preferredHeight + _header.rect.height;

        _rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            targetHeight);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }
}
