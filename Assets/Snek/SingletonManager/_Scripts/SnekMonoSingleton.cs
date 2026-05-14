using Snek.Utilities;
using UnityEngine;

namespace Snek.SingletonManager
{
    public abstract class SnekMonoSingleton : SnekMonoBehaviour
    {
        public void RunInitialization()
        {
            _isValid = true;

            Initialize();
            Validate();

            if (!_isValid)
            {
                Debug.LogError(InvalidSetupMessage(name), this);

                OnFailValidation();

                gameObject.SetActive(false);
            }
            else
                OnInitializationSuccess();
        }

        protected override bool IsManuallyInitialized()
        {
            return true;
        }
    }
}