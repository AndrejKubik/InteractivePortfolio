using System;
using Snek.GameUI;
using Snek.Utilities;

[UseSnekInspector]
public class PortfolioProjectVideoDemoTimeline : SnekUISlider, ISnekInitializableExternal<PortfolioProjectVideoDemoTimeline.Data>
{
    public readonly struct Data
    {
        public readonly Action<float> OnUserMoveSlider;

        public Data(Action<float> onUserMoveSlider)
        {
            OnUserMoveSlider = onUserMoveSlider;
        }
    }

    private Action<float> _onUserMoveSlider = null;

    public void OnBeforeInitialize(Data data)
    {
        _onUserMoveSlider = data.OnUserMoveSlider;
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
