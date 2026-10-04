using System;
using Snek.GameUI;
using Snek.Utilities;

[UseSnekInspector]
public class PortfolioProjectVideoDemoTimeline : SnekUISlider, ISnekInitializableWithData<PortfolioProjectVideoDemoTimeline.Data>
{
    public delegate void OnUserInteractCallback(float sliderValue, bool isReleaseInput);

    public readonly struct Data
    {
        public readonly OnUserInteractCallback OnUserMoveSlider;

        public Data(OnUserInteractCallback onUserMoveSlider)
        {
            OnUserMoveSlider = onUserMoveSlider;
        }
    }

    private OnUserInteractCallback _onUserMoveSlider = null;

    public void PrepareInitializationData(Data data)
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
        _onUserMoveSlider.Invoke(newValue, false);
    }

    protected override void OnHandleRelease()
    {
        _onUserMoveSlider.Invoke(GetValue(), true);
    }
}
