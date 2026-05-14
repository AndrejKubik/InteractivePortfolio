using Snek.Utilities;
using UnityEditor;

namespace SnekEditor.GameBootstrapper
{
    [SnekAutoGenerateAsset("Assets/Snek/GameBootstrapper", "Cache")]
    [UseSnekInspector]
    public class SnekGameBootstrapperInspectorCache : SnekScriptableObject
    {
        public SceneAsset StartScene;
    }
}
