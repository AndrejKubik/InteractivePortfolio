using System;
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
        protected bool _isValid { get; private set; }
        protected bool _isInitializedOnce { get; private set; }

        private List<SnekEssentialComponentReference> _essentialComponents = new();
        private List<SnekMonoSubcomponent> _subcomponents = new();

        protected virtual void Awake()
        {
            if (!IsManuallyInitialized() && !IsInitializedInStart())
                Initialize(false, false);
        }

        protected virtual void Start()
        {
            if (!IsManuallyInitialized() && IsInitializedInStart())
                Initialize(false, false);
        }

        private void OnDestroy()
        {
            OnDispose();

            foreach (SnekMonoSubcomponent subcomponent in _subcomponents)
                subcomponent.Dispose();
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
            return this is ISnekInitializableManual;
        }

        public void Initialize<TData>(TData data)
        {
            if (this is ISnekInitializableWithData<TData> initializable)
            {
                initializable.PrepareInitializationData(data);

                Initialize(true, true);
            }
            else
                Debug.LogError(
                    $"{GetType().Name} is not of type {nameof(ISnekInitializableWithData<TData>)}.\n" +
                    $"Cannot initialize with data.");
        }

        public void Initialize()
        {
            Initialize(true, false);
        }

        private void Initialize(bool isManualInitialization, bool isDataPrepared)
        {
            if (isManualInitialization && IsDataRequiredForInitialization() && !isDataPrepared) //fix this
            {
                Debug.LogError(
                    $"This component requires data for initialization, use Initialize(TData data) overload instead.\n" +
                    $"Type: {GetType().Name}");

                return;
            }

            _isValid = true;

            OnInitialize();
            ValidateEssentialComponents();

            if (_isValid)
                Validate();

            if (!_isValid)
            {
                Debug.LogError(GetInvalidSetupMessage(), gameObject);

                OnFailValidation();

                gameObject.SetActive(false);
            }
            else
            {
                OnInitializationSuccess();

                _isInitializedOnce = true;
            }
        }

        private bool IsDataRequiredForInitialization()
        {
            return this is ISnekInitializableManual manualInitializable && manualInitializable.IsDataRequiredForInitialization();
        }

        protected void InitializeSubcomponent<T>(out T subcomponent) where T : SnekMonoSubcomponent
        {
            subcomponent = new SnekMonoSubcomponent() as T;

            subcomponent.Initialize(this);

            _subcomponents.Add(subcomponent);
        }

        protected void InitializeSubcomponent<T, TData>(out T subcomponent, TData data) where T : SnekMonoSubcomponent, new()
        {
            subcomponent = new T();

            if (subcomponent is ISnekInitializableWithData<TData> initializable)
            {
                initializable.PrepareInitializationData(data);
                subcomponent.Initialize(this);

                _subcomponents.Add(subcomponent);
            }
            else
            {
                subcomponent = null;

                Debug.LogError(
                    $"{typeof(T).Name} is not of type {nameof(ISnekInitializableWithData<TData>)}.\n" +
                    $"Cannot initialize with data.");
            }
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
        /// Use this to check if an Object type reference is populated, works only in <c>Validate()</c> method
        /// </summary>
        protected void ValidateEssentialComponent<T>(T value, string name, bool nicifyName = true) where T : UnityEngine.Object
        {
            if (nicifyName)
                name = name.TrimStart('_').Nicify();

            if (value == null)
                FailValidation($"<b>{name}</b> is not assigned.");
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

        /// <summary>
        /// Prints an error message in the console while making sure the <c>OnFailInitialization()</c> instead of <c>OnInitializationSuccess()</c> is called after <c>Validate()</c>, 
        /// </summary>
        protected void FailValidation(string message)
        {
            _isValid = false;

            Debug.LogError(message, gameObject);
        }

        private string GetInvalidSetupMessage()
        {
            return $"Component setup invalid, disabling game object <b>[{name}]</b>";
        }

        /// <summary>
        /// Use for getting component references through code
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Use for checking if data setup is correct, relevant methods for validation: <c>ValidateEssentialComponent()</c>, <c>FailValidation()</c>
        /// </summary>
        protected virtual void Validate() { }

        /// <summary>
        /// Use for custom logic right before GameObject gets disabled in addition to error logs in the developer console
        /// </summary>
        protected virtual void OnFailValidation() { }

        /// <summary>
        /// Use this method as the logic entry point instead of Start() or Awake() methods
        /// </summary>
        protected virtual void OnInitializationSuccess() { }

        /// <summary>
        /// Called when the component instance(or its game object) is destroyed
        /// </summary>
        protected virtual void OnDispose() { }
    }
}