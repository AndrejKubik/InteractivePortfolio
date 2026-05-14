using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

namespace Snek.SingletonManager
{
    [UseSnekInspector]
    public class SnekSingletonManager : SnekMonoBehaviour
    {
        public List<SnekMonoSingleton> Singletons;
        public List<SnekMonoSingleton> SingletonPrefabs;

        private static SnekSingletonManager _instance;

        private static bool _isInstanceCreationComplete = false;

        protected override void Initialize()
        {
            if (_instance != null)
                Destroy(gameObject);

            _instance = this;
            _isInstanceCreationComplete = false;
        }

        protected override void Validate()
        {
            if (IsAnyPrefabReferenceMissing())
                FailValidation("Found missing references in singleton prefabs list.");
        }

        protected override void OnInitializationSuccess()
        {
            CreateSingletonInstances();
            InitializeSingletonInstances();
            DontDestroyOnLoad(this);
        }

        private bool IsAnyPrefabReferenceMissing()
        {
            foreach (SnekMonoSingleton prefab in SingletonPrefabs)
                if (prefab == null)
                    return true;

            return false;
        }

        private void CreateSingletonInstances()
        {
            Singletons = new List<SnekMonoSingleton>();

            foreach (SnekMonoSingleton prefab in SingletonPrefabs)
                CreateSingletonInstance(prefab);

            _isInstanceCreationComplete = true;
        }

        private void InitializeSingletonInstances()
        {
            foreach (SnekMonoSingleton singletonInstance in Singletons)
                singletonInstance.RunInitialization();
        }

        private void CreateSingletonInstance(SnekMonoSingleton prefab)
        {
            SnekMonoSingleton singletonInstance = Instantiate(prefab, transform);
            singletonInstance.name = prefab.name;

            Singletons.Add(singletonInstance);
        }

        public static T GetSingleton<T>() where T : SnekMonoSingleton
        {
            if (!_isInstanceCreationComplete)
            {
                Debug.LogError(
                    "Trying to get a singleton reference before its instance is created.\n" +
                    "Make sure to use a bootstrapper, disable dependencies until all instances are created or initialize dependencies through Start().");

                return null;
            }

            foreach (SnekMonoSingleton singleton in _instance.Singletons)
                if (singleton is T instance)
                    return instance;

            Debug.LogError($"Cannot find singleton of type {typeof(T)}.");

            return null;
        }
    }
}