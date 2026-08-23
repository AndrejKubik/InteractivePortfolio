using System;
using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectVideoDemoTimeline : SnekUISlider
{
    private Action<float> _onUserMoveSlider;

    protected override bool IsManuallyInitialized()
    {
        return true;
    }

    public void InitializeExternally(Action<float> onUserMoveSlider)
    {
        _onUserMoveSlider = onUserMoveSlider;

        RunInitialization();
    }

    protected override void Validate()
    {
        base.Validate();

        if (_onUserMoveSlider == null)
            FailValidation("Slider click callback not assigned.");
    }

    protected override void OnSliderMove(float newValue)
    {
        _onUserMoveSlider.Invoke(newValue);
    }
}
