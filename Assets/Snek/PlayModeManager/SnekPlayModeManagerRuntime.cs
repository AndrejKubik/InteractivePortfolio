using System;

namespace Snek.PlayModeManager
{
    public static class SnekPlayModeManagerRuntime
    {
        internal static Action OnRuntimePlayModeExitRequest;

        public static void RequestPlayModeExit()
        {
            OnRuntimePlayModeExitRequest?.Invoke();
        }
    }
}
