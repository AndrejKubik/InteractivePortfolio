using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class PortfolioProjectVideoDemoPlayPauseButton : SnekUIButton
{
    [SerializeField] private Image _symbolImage;

    protected override void Validate()
    {
        base.Validate();

        ValidateEssentialComponent(_symbolImage, nameof(_symbolImage));
    }

    public void SetSymbol(Sprite newSymbol)
    {
        _symbolImage.sprite = newSymbol;
    }

    public void SetSymbolAlpha(float newAlpha)
    {
        _symbolImage.color = _symbolImage.color.ChangeAlpha(newAlpha);
    }
}
