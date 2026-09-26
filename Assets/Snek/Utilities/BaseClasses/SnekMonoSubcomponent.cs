using UnityEngine;

namespace Snek.Utilities
{
    public class SnekMonoSubcomponent
    {
        protected SnekMonoBehaviour _parentComponent { get; private set; }
        protected bool _isValid { get; private set; }
        protected bool _isInitializedOnce { get; private set; }

        public void Initialize<TData>(TData data, SnekMonoBehaviour parentComponent)
        {
            if (this is ISnekInitializableWithData<TData> initializable)
            {
                initializable.PrepareInitializationData(data);

                Initialize(parentComponent);
            }
            else
                Debug.LogError(
                    $"{GetType().Name} is not of type {nameof(ISnekInitializableWithData<TData>)}.\n" +
                    $"Cannot initialize externally.");
        }

        public void Initialize(SnekMonoBehaviour parentComponent)
        {
            if (parentComponent == null)
            {
                _isValid = false;

                Debug.LogError($"Parent component of subcomponent is not assigned, cannot initialize. ({this})");

                return;
            }

            _parentComponent = parentComponent;

            _isValid = true;

            OnInitialize();

            if (_isValid)
                Validate();

            if (!_isValid)
            {
                Debug.LogError(GetInvalidSetupMessage(), parentComponent.gameObject);

                OnFailValidation();

                parentComponent.gameObject.SetActive(false);
            }
            else
            {
                OnInitializationSuccess();

                _isInitializedOnce = true;
            }
        }

        

        protected virtual void OnInitialize()
        {

        }

        protected virtual void Validate()
        {

        }

        protected virtual void OnInitializationSuccess()
        {

        }

        protected virtual void OnFailValidation()
        {

        }

        protected void FailValidation(string message)
        {
            _isValid = false;

            Debug.LogError(message, _parentComponent.gameObject);
        }

        private string GetInvalidSetupMessage()
        {
            return $"Subcomponent setup invalid, disabling game object <b>[{_parentComponent.name}]</b>";
        }
    }
}