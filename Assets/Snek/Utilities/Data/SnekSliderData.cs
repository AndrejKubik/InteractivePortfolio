using System;

namespace Snek.Utilities
{
    [Serializable]
    public struct SnekSliderData
    {
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;

        public bool UseWholeNumbers;
    }
}
