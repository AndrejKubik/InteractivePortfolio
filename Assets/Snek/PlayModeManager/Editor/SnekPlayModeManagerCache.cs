using Snek.Utilities;
using SnekEditor.GUIUtilities;
using UnityEditor.SceneManagement;

namespace SnekEditor.PlayModeManager
{
    [SnekAutoGenerateAsset("Assets/Snek/PlayModeManager/Editor/", "Cache")]
    [UseSnekInspector(true)]
    public class SnekPlayModeManagerCache : SnekScriptableObject
    {
        [SnekInspectOnly] public bool IsSwitchingToCorrectScene;
        [SnekInspectOnly] public SceneSetup[] CachedSceneSetup;
    }
}