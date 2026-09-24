using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class VideoPlayerToggleFullScreenButton : SnekUIButton
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
}
