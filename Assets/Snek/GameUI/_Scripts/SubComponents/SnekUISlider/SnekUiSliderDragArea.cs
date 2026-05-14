using System;

namespace Snek.GameUI
{
    [Serializable]
    public readonly struct SnekUiSliderDragArea
    {
        public readonly float MinValue;
        public readonly float MaxValue;

        public SnekUiSliderDragArea(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
