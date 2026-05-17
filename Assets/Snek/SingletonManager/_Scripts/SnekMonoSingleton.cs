using Snek.Utilities;
using UnityEngine;

namespace Snek.SingletonManager
{
    public abstract class SnekMonoSingleton : SnekMonoBehaviour
    {
        protected override bool IsManuallyInitialized()
        {
            return true;
        }
    }
}