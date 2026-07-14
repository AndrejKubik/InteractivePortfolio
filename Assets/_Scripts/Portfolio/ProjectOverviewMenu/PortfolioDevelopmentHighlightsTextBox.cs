using Snek.SingletonManager;
using Snek.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioDevelopmentHighlightsTextBox : SnekMonoBehaviour
{
    private EventManager _eventManager;

    private RectTransform _rectTransform;
    
    [SerializeField] private RectTransform _sectionParent;
    [SerializeField] private RectTransform _title;
    [SerializeField] private TextMeshProUGUI _textMesh;

    protected override void Initialize()
    {
        _eventManager = SnekSingletonManager.GetSingleton<EventManager>();

        GetEssentialComponent(out _rectTransform);
    }

    protected override void Validate()
    {
        if (!_eventManager)
            FailValidation("Cannot find event manager singleton.");

        if (!_sectionParent)
            FailValidation("Section parent not assigned.");

        if (!_title)
            FailValidation("Title not assigned.");

        if (!_textMesh)
            FailValidation("Text mesh not assigned.");
    }

    public void ApplyData(string text, float fontSize)
    {
        if (string.IsNullOrEmpty(text) || fontSize <= 0f)
        {
            Debug.LogError("Provided Development highlights text box data is invalid, cannot apply data.");

            _eventManager.RequestShowAllProjects();

            return;
        }

        _textMesh.SetText(text);

        _textMesh.fontSize = fontSize;

        AdjustDevelopmentHighlightsTextBoxHeight();
    }

    private void AdjustDevelopmentHighlightsTextBoxHeight()
    {
        Vector2 preferredSize = _textMesh.GetPreferredValues(
            _textMesh.text,
            _rectTransform.rect.width,
            Mathf.Infinity);

        _sectionParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y + _title.rect.size.y);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_sectionParent);
    }
}
