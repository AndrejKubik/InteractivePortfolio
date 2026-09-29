namespace Snek.Utilities
{
    public interface ISnekInitializableWithData<TData> : ISnekInitializableManual
    {
        public void PrepareInitializationData(TData data);

        bool ISnekInitializableManual.IsDataRequiredForInitialization()
        {
            return true;
        }
    }
}