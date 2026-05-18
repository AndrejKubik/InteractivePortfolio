using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectButton : SnekUIButton, IInfiniteCarouselElement
{
    public RectTransform GetRectTransform()
    {
        return _button.targetGraphic.rectTransform;
    }
}
