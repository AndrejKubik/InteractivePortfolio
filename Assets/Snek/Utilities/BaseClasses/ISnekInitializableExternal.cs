using UnityEngine;

namespace Snek.Utilities
{
    public interface ISnekInitializableExternal<TData> : ISnekInitializableManual
    {
        public void RunInitialization(TData data)
        {
            if(!IsSnekMonoBehaviour(out SnekMonoBehaviour behaviour))
            {
                Debug.LogError($"Only child classes of SnekMonoBehaviour type can use Snek external initialization interface.");

                return;
            }

            OnBeforeInitialize(data);

            behaviour.RunInitialization();
        }

        private bool IsSnekMonoBehaviour(out SnekMonoBehaviour snekMonoBehaviour)
        {
            snekMonoBehaviour = this is SnekMonoBehaviour behaviour ?
                behaviour : null;

            return snekMonoBehaviour != null;
        }

        public void OnBeforeInitialize(TData data);
    }
}