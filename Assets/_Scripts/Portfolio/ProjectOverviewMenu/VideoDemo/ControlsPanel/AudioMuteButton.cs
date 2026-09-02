using System;
using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

[UseSnekInspector]
public class AudioMuteButton : SnekUIButton
{
    private const float HalfVolumeThreshold = 0.4f;

    [SerializeField] private Image _symbol;

    [Space(10f)]
    [SerializeField] private Sprite _mutedSprite;
    [SerializeField] private Sprite _halfVolumeSprite;
    [SerializeField] private Sprite _fullVolumeSprite;

    protected override void Validate()
    {
        base.Validate();

        ValidateEssentialComponent(_symbol, nameof(_symbol));

        ValidateEssentialComponent(_mutedSprite, nameof(_mutedSprite));
        ValidateEssentialComponent(_halfVolumeSprite, nameof(_halfVolumeSprite));
        ValidateEssentialComponent(_fullVolumeSprite, nameof(_fullVolumeSprite));
    }

    public void MatchSpriteWithVolume(float newVolume)
    {
        if (newVolume <= 0f)
            SetSymbolSprite(_mutedSprite);
        else if (newVolume < HalfVolumeThreshold)
            SetSymbolSprite(_halfVolumeSprite);
        else
            SetSymbolSprite(_fullVolumeSprite);
    }

    public void SetMutedSymbol()
    {
        SetSymbolSprite(_mutedSprite);
    }

    private void SetSymbolSprite(Sprite sprite)
    {
        _symbol.sprite = sprite;
    }
}
