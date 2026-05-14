using Snek.GameBootstrapper;
using SnekEditor.GUIUtilities;
using SnekEditor.ScriptableObjectManager;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GameBootstrapper
{
    [CustomEditor(typeof(SnekGameBootstrapper))]
    public class SnekGameBootstrapperInspector : SnekMonoBehaviourInspectorCustom<SnekGameBootstrapper>
    {
        private const float StartScenePropertyWidth = 200f;
        private const float StartScenePropertyHeight= 25f;

        private GUIStyle _labelStyle;

        private SerializedProperty sp_StartSceneName;

        private SnekGameBootstrapperInspectorCache _cache;
        private SerializedObject so_Cache;
        private SerializedProperty sp_StartScene;

        protected override void OnCreateInspectorInstance()
        {
            sp_StartSceneName = serializedObject.FindProperty(nameof(SnekGameBootstrapper.StartSceneName));

            _cache = SnekScriptableObjectManager.GetAsset<SnekGameBootstrapperInspectorCache>();
            so_Cache = new SerializedObject(_cache);
            sp_StartScene = so_Cache.FindProperty(nameof(_cache.StartScene));

            UpdateStartSceneName();
        }

        protected override bool Initialize()
        {
            if (!InitializeLabelStyle())
                return false;

            return base.Initialize();
        }

        private bool InitializeLabelStyle()
        {
            if (_labelStyle == null)
                _labelStyle = SnekGUIStyles.Label(16, stretchHeight: true);

            return _labelStyle != null;
        }

        protected override void DrawContent()
        {
            if (_cache == null)
            {
                EditorGUILayout.PropertyField(sp_StartSceneName);

                return;
            }

            so_Cache.Update();

            var mainScope = new SnekGUIHorizontalScope(
                SnekGUIScopeAnchor.Center,
                GUILayout.Height(StartScenePropertyHeight));

            using (mainScope)
            {
                GUILayout.Label("Start Scene:", _labelStyle);

                GUILayout.Space(20f);

                EditorGUILayout.PropertyField(
                    sp_StartScene,
                    GUIContent.none,
                    GUILayout.Width(StartScenePropertyWidth),
                    GUILayout.ExpandHeight(true));
            }

            if (so_Cache.ApplyModifiedProperties())
                UpdateStartSceneName();
        }

        private void UpdateStartSceneName()
        {
            sp_StartSceneName.stringValue = _cache.StartScene == null ?
                string.Empty : _cache.StartScene.name;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
