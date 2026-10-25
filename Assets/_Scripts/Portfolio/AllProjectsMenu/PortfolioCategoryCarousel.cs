using Snek.EndlessCarousel;
using Snek.Utilities;

[UseSnekInspector]
public class PortfolioCategoryCarousel : SnekEndlessCarousel, ISnekInitializableWithData<PortfolioCategoryCarousel.Data>
{
    public readonly struct Data
    {
        public readonly float ElementWidth;
        public readonly float ElementHeight;

        public Data(float elementWidth, float elementHeight)
        {
            ElementWidth = elementWidth;
            ElementHeight = elementHeight;
        }
    }

    public void PrepareInitializationData(Data data)
    {
        ElementWidth = data.ElementWidth;
        ElementHeight = data.ElementHeight;
    }
}
