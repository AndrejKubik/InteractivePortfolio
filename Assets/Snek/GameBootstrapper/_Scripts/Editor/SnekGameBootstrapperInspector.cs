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
        private const float StartScenePropertyHeight = 25f;

        private GUIStyle _labelStyle;

        private SerializedProperty sp_StartSceneName;
        private SerializedProperty sp_PreLaunchServices;

        private SnekGameBootstrapperInspectorCache _cache;
        private SerializedObject so_Cache;
        private SerializedProperty sp_StartScene;

        private SnekReorderableList list_PreLaunchServices;

        protected override void OnCreateInspectorInstance()
        {
            sp_StartSceneName = serializedObject.FindProperty(nameof(SnekGameBootstrapper.StartSceneName));
            sp_PreLaunchServices = serializedObject.FindProperty(nameof(SnekGameBootstrapper.PreLaunchServices));

            _cache = SnekScriptableObjectManager.GetAsset<SnekGameBootstrapperInspectorCache>();
            so_Cache = new SerializedObject(_cache);
            sp_StartScene = so_Cache.FindProperty(nameof(_cache.StartScene));

            list_PreLaunchServices = new SnekReorderableList(serializedObject, sp_PreLaunchServices);

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

            DrawStartScenePropertyField();

            GUILayout.Space(10f);

            list_PreLaunchServices.Draw();

            if (so_Cache.ApplyModifiedProperties())
                UpdateStartSceneName();
        }

        private void DrawStartScenePropertyField()
        {
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
        }

        private void UpdateStartSceneName()
        {
            sp_StartSceneName.stringValue = _cache.StartScene == null ?
                string.Empty : _cache.StartScene.name;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
