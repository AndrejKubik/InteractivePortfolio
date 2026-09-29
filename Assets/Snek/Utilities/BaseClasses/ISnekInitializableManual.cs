namespace Snek.Utilities
{
    public interface ISnekInitializableManual
    {
        public bool IsDataRequiredForInitialization()
        {
            return false;
        }
    }
}