namespace Snek.Utilities
{
    public interface ISnekInitializableWithData<TData> : ISnekInitializableManual
    {
        public void RunInitialization(TData data)
        {
            PrepareInitializationData(data);

            if (IsSnekMonoBehaviour(out SnekMonoBehaviour behaviour))
                behaviour.Initialize();
        }

        public void PrepareInitializationData(TData data);

        private bool IsSnekMonoBehaviour(out SnekMonoBehaviour snekMonoBehaviour)
        {
            snekMonoBehaviour = this is SnekMonoBehaviour behaviour ?
                behaviour : null;

            return snekMonoBehaviour != null;
        }
    }
}