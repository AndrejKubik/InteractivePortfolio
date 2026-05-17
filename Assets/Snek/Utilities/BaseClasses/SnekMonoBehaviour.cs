using System.Collections.Generic;
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

        private List<SnekEssentialComponentReference> _essentialComponents = new List<SnekEssentialComponentReference>();

        public void RunInitialization()
        {
            _isValid = true;

            Initialize();
            ValidateEssentialComponents();

            if (_isValid)
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

        protected virtual void Awake()
        {
            if (!IsManuallyInitialized() && !IsInitializedInStart())
                RunInitialization();
        }

        protected virtual void Start()
        {
            if (!IsManuallyInitialized() && IsInitializedInStart())
                RunInitialization();
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

        private void ValidateEssentialComponents()
        {
            foreach (SnekEssentialComponentReference essentialComponent in _essentialComponents)
                if (essentialComponent.Reference == null)
                {
                    string componentName = essentialComponent.Type.Name.Nicify();

                    FailValidation($"Cannot find <b>[{componentName}]</b> component.");
                }
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

        protected void GetEssentialComponent<T>(out T componentReference, SnekGetComponentContext searchContext = SnekGetComponentContext.Self) where T : Component
        {
            componentReference = searchContext switch
            {
                SnekGetComponentContext.Self => GetComponent<T>(),
                SnekGetComponentContext.Children => GetComponentInChildren<T>(),
                SnekGetComponentContext.Parents => GetComponentInParent<T>(),
                _ => GetComponent<T>(),
            };

            var essentialReference = new SnekEssentialComponentReference(typeof(T), componentReference);

            _essentialComponents.Add(essentialReference);
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