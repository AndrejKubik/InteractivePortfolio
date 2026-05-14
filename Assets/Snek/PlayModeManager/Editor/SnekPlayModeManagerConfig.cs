using Snek.Utilities;
using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.PlayModeManager
{
    [SnekAutoGenerateAsset("Assets/Snek/PlayModeManager/Editor/", "Config")]
    [UseSnekInspector(true)]
    public class SnekPlayModeManagerConfig : SnekScriptableObject
    {
        [SerializeField] private SceneAsset _defaultScene;
        [SerializeField] private bool _forceDefaultScene;

        public string GetDefaultScenePath()
        {
            if (!_defaultScene)
                return string.Empty;

            return AssetDatabase.GetAssetPath(_defaultScene);
        }

        public string GetDefaultSceneName()
        {
            if (!_defaultScene)
                return string.Empty;

            return _defaultScene.name;
        }

        public SceneAsset GetDefaultScene()
        {
            return _defaultScene;
        }

        public void SetDefaultScene(SceneAsset sceneAsset)
        {
            _defaultScene = sceneAsset;
        }

        public bool IsFeatureEnabled()
        {
            return _forceDefaultScene;
        }

        public void EnableFeature(bool newState)
        {
            _forceDefaultScene = newState;
        }
    }
}