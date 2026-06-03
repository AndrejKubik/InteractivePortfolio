using Snek.EndlessCarousel;
using Snek.GameUI;
using Snek.Utilities;
using TMPro;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectButton : SnekUIButton, ISnekEndlessCarouselElement
{
    protected override void OnInitializationSuccess()
    {
        GetComponentInChildren<TextMeshProUGUI>(true).text = transform.GetSiblingIndex().ToString();
    }

    public RectTransform GetRectTransform()
    {
        return _button.targetGraphic.rectTransform;
    }
}
