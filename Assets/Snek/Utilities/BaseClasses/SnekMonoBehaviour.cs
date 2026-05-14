using UnityEngine;

namespace Snek.Utilities
{
    /// <summary>
    /// <list type="bullet">MonoBehavior class with prepared Initialization and Validation logic within <c>Awake()</c></list>
    /// <list type="bullet">Assign data/references to variables in <c>Initialize()</c></list>
    /// <list type="bullet">Use <c>FailValidation()</c> in <c>Validate()</c> to disable the GameObject and print a custom error message</list>
    /// </summary>
    public abstract class SnekMonoBehaviour : MonoBehaviour
    {
        protected bool _isValid;

        protected virtual void Awake()
        {
            if (IsManuallyInitialized() || IsInitializedInStart())
                return;

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

        protected virtual void Start()
        {
            if (IsManuallyInitialized() || !IsInitializedInStart())
                return;

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

        /// <summary>
        /// <list type="bullet"><c>True</c> = you can completely override the <c>Awake()</c></list>
        /// <list type="bullet"><c>False</c> = you can completely override <c>Start()</c></list> 
        /// </summary>
        protected virtual bool IsInitializedInStart()
        {
            return false;
        }

        protected virtual bool IsManuallyInitialized()
        {
            return false;
        }

        /// <summary>
        /// Use for getting components through code, called in <c>Awake()</c> or <c>Start()</c> before <c>Validate()</c>
        /// </summary>
        protected virtual void Initialize()
        {

        }

        /// <summary>
        /// Use for checking if data setup is correct, called in <c>Awake()</c> or <c>Start()</c> after <c>Initialize()</c>
        /// </summary>
        protected virtual void Validate()
        {

        }

        /// <summary>
        /// Use for custom logic right before GameObject gets disabled in addition to error logs in the developer console
        /// </summary>
        protected virtual void OnFailValidation()
        {

        }

        /// <summary>
        /// Called in <c>Awake()</c> or <c>Start()</c> after <c>Validate()</c> if it was successful
        /// </summary>
        protected virtual void OnInitializationSuccess()
        {

        }

        protected void FailValidation(string message)
        {
            _isValid = false;
            Debug.LogError(message);
        }

        protected string InvalidSetupMessage(string gameObjectName)
        {
            return $"Component setup invalid, disabling game object <b>[{gameObjectName}]</b>";
        }
    }
}