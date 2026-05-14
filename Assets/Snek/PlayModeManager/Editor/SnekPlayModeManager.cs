using Snek.PlayModeManager;
using SnekEditor.GUIUtilities;
using SnekEditor.ScriptableObjectManager;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SnekEditor.PlayModeManager
{
    [InitializeOnLoad]
    public static class SnekPlayModeManager
    {
        private static SnekPlayModeManagerConfig _config;
        private static SnekPlayModeManagerCache _cache;

        static SnekPlayModeManager()
        {
            SnekScriptableObjectManager.CallAfterInitialization(InitializeFiles);

            SnekPlayModeManagerRuntime.OnRuntimePlayModeExitRequest = OnRuntimeExitPlayModeRequest;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private static void OnPlayModeStateChange(PlayModeStateChange newState)
        {
            if (newState == PlayModeStateChange.ExitingEditMode)
                OnPlayModeStart();
            else if (newState == PlayModeStateChange.EnteredEditMode)
                OnPlayModeStop();
        }

        public static void OnPlayModeStart()
        {
            if (_cache.IsSwitchingToCorrectScene)
                return;

            ApplyConfigToStartup();
        }

        public static void OnPlayModeStop()
        {
            if (_cache.IsSwitchingToCorrectScene || !IsSceneSetupChanged())
                return;

            RestoreSceneSetupBeforePlayMode();
        }

        private static bool IsSceneSetupChanged()
        {
            return _cache.CachedSceneSetup != null && _cache.CachedSceneSetup.Length > 0;
        }

        [InitializeOnEnterPlayMode]
        public static void InitializeFiles()
        {
            if (!_config)
                _config = SnekScriptableObjectManager.GetAsset<SnekPlayModeManagerConfig>();

            if (!_cache)
                _cache = SnekScriptableObjectManager.GetAsset<SnekPlayModeManagerCache>();
        }

        public static SnekPlayModeManagerConfig GetConfigFile()
        {
            return _config;
        }

        private static void ApplyConfigToStartup()
        {
            if (_config.IsFeatureEnabled() && !IsDefaultSceneAssigned())
                OnMissingDefaultSceneReferencePlayModeStart();

            if (!_config.IsFeatureEnabled())
                return;

            if (GetActiveSceneName() != _config.GetDefaultSceneName())
                OnWrongScenePlayModeStart();
        }

        private static void OnMissingDefaultSceneReferencePlayModeStart()
        {
            EditorApplication.Beep();

            using (new SnekDirectValueChangeScope(_config))
                _config.EnableFeature(false);

            Debug.LogError(
                "Assigned default scene reference is missing, please assign a valid scene.\n" +
                "Play Mode Start Scene Manager Disabled. Entering Play Mode normally...");
        }

        private static void OnWrongScenePlayModeStart()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                SnekPlayModeStopper.ExitPlayMode(true, OnPlayModeCancel);

                return;
            }

            Debug.Log(
                $"Wrong scene opened, preventing play mode...\n" +
                $"Scene: {GetActiveSceneName()}");

            _cache.IsSwitchingToCorrectScene = true;
            _cache.CachedSceneSetup = EditorSceneManager.GetSceneManagerSetup();

            SnekPlayModeStopper.ExitPlayMode(false, LoadDefaultSceneAndStartPlayMode);
        }

        private static void OnPlayModeCancel()
        {
            SnekGUIUtility.ShowEditorAlert(
                "Play Mode Prevented",
                "Prevented Play Mode to preserve unsaved changes.");
        }

        private static void LoadDefaultSceneAndStartPlayMode()
        {
            Debug.Log(
                $"Restarting play mode with correct scene...\n" +
                $"Scene: {_config.GetDefaultSceneName()}");

            EditorSceneManager.OpenScene(_config.GetDefaultScenePath(), OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();

            _cache.IsSwitchingToCorrectScene = false;
        }

        private static string GetActiveSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        private static bool IsDefaultSceneAssigned()
        {
            return _config.GetDefaultScene() != null;
        }

        private static void RestoreSceneSetupBeforePlayMode()
        {
            EditorSceneManager.RestoreSceneManagerSetup(_cache.CachedSceneSetup);
            _cache.CachedSceneSetup = null;

            SnekGUIUtility.ShowEditorAlert(
                "Open Scenes Restored",
                "Restored open scenes from before starting Play Mode.");
        }

        private static void OnRuntimeExitPlayModeRequest()
        {
            SnekPlayModeStopper.ExitPlayMode(true);

            SnekGUIUtility.ShowEditorAlert(
                "Play Mode Stopped",
                "Exited play mode on user request");
        }

        [MenuItem(SnekEditorUtility.MenuItemRoot + "Play Mode Manager")]
        private static void ShowToolWindow()
        {
            EditorWindow.GetWindow<SnekPlayModeManagerWindow>(true);
        }
    }
}