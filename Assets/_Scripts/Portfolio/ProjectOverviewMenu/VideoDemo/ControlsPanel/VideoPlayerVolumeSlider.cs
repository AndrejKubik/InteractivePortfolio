using System;
using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class VideoPlayerVolumeSlider : SnekUISlider, ISnekInitializableExternal<VideoPlayerVolumeSlider.Data>
{
    public readonly struct Data
    {
        public readonly Action<float> OnValueChange;

        public Data(Action<float> onValueChange)
        {
            OnValueChange = onValueChange;
        }
    }

    private Action<float> _onValueChange;

    public void OnBeforeInitialize(Data data)
    {
        _onValueChange = data.OnValueChange;
    }

    protected override void Validate()
    {
        base.Validate();

        if (_onValueChange == null)
            FailValidation("Slider value change callback not assigned.");
    }

    protected override void OnSliderMove(float newValue)
    {
        _onValueChange.Invoke(newValue);
    }
}
