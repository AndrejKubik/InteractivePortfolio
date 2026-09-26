using Snek.Utilities;
using UnityEngine.Video;

public class PortfolioProjectVideoPlayerController : SnekMonoSubcomponent, ISnekInitializableWithData<PortfolioProjectVideoPlayerController.Data>
{
    public readonly struct Data
    {
        public readonly VideoPlayer VideoPlayer;
    }

    private VideoPlayer _videoPlayer;

    public void PrepareInitializationData(Data data)
    {
        _videoPlayer = data.VideoPlayer;
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_videoPlayer, nameof(_videoPlayer));
    }
}
