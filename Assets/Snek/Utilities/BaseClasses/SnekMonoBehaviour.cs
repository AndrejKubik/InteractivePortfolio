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

        public void Initialize<TData>(TData data)
        {
            if (this is ISnekInitializableWithData<TData> initializable)
            {
                initializable.PrepareInitializationData(data);

                Initialize();
            }
            else
                Debug.LogError(
                    $"{GetType().Name} is not of type {nameof(ISnekInitializableWithData<TData>)}.\n" +
                    $"Cannot initialize with data.");
        }

        public void Initialize()
        {
            if(this is ISnekInitializableManual manualInitializable && manualInitializable.IsDataRequiredForInitialization())
            {
                Debug.LogError("This component requires data for initialization, use Initialize(TData data) overload instead.");

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

        protected virtual void Awake()
        {
            if (!IsManuallyInitialized() && !IsInitializedInStart())
                Initialize();
        }

        protected virtual void Start()
        {
            if (!IsManuallyInitialized() && IsInitializedInStart())
                Initialize();
        }

        private void OnDestroy()
        {
            OnDispose();

            foreach (SnekMonoSubcomponent subcomponent in _subcomponents)
                subcomponent.Dispose();
        }

        protected virtual void OnDispose()
        {

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

        protected void InitializeSubcomponent<T>(out T subcomponent) where T : SnekMonoSubcomponent
        {
            subcomponent = new SnekMonoSubcomponent() as T;

            subcomponent.Initialize(this);

            _subcomponents.Add(subcomponent);
        }

        protected void InitializeSubcomponent<T, TData>(out T subcomponent, TData data) where T : SnekMonoSubcomponent, new()
        {
            subcomponent = new T();

            if(subcomponent is ISnekInitializableWithData<TData> initializable)
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

        /// <summary>
        /// Use for getting components through code, called in <c>Awake()</c> or <c>Start()</c> before <c>Validate()</c>
        /// </summary>
        protected virtual void OnInitialize()
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

        protected void ValidateEssentialComponent<T>(T value, string name, bool nicifyName = true) where T : UnityEngine.Object
        {
            if (nicifyName)
                name = name.TrimStart('_').Nicify();

            if (value == null)
                FailValidation($"<b>{name}</b> is not assigned.");
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

            Debug.LogError(message, gameObject);
        }

        private string GetInvalidSetupMessage()
        {
            return $"Component setup invalid, disabling game object <b>[{name}]</b>";
        }
    }
}