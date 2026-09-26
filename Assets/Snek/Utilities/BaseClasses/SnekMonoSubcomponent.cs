using UnityEngine;

namespace Snek.Utilities
{
    public class SnekMonoSubcomponent
    {
        protected SnekMonoBehaviour _parentComponent { get; private set; }
        protected bool _isValid { get; private set; }
        protected bool _isInitializedOnce { get; private set; }

        internal void Initialize(SnekMonoBehaviour parentComponent)
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
                Debug.LogError(GetInvalidSetupMessage(), _parentComponent.gameObject);

                OnFailValidation();

                _parentComponent.gameObject.SetActive(false);
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

        protected void ValidateEssentialComponent<T>(T value, string name, bool nicifyName = true) where T : Object
        {
            if (nicifyName)
                name = name.TrimStart('_').Nicify();

            if (value == null)
                FailValidation($"<b>{name}</b> is not assigned.");
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

        internal virtual void OnDispose()
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