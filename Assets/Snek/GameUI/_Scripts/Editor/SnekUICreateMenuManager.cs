using SnekEditor.ScriptableObjectManager;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GameUI
{
    public static class SnekUICreateMenuManager
    {
        private const string MenuItemRoot = "GameObject/Snek Game UI/";

        [MenuItem(MenuItemRoot + "Button")]
        private static void CreateButton(MenuCommand menuCommand)
        {
            if (FindPrefabContainer(out SnekUIPrefabContainer prefabContainer))
                SnekEditorPrefabUtility.InstantiatePrefabByReference(prefabContainer.Button, menuCommand, true);
        }

        [MenuItem(MenuItemRoot + "Slider")]
        private static void CreateSlider(MenuCommand menuCommand)
        {
            if (FindPrefabContainer(out SnekUIPrefabContainer prefabContainer))
                SnekEditorPrefabUtility.InstantiatePrefabByReference(prefabContainer.Slider, menuCommand, true);
        }

        [MenuItem(MenuItemRoot + "Switch")]
        private static void CreateSwitch(MenuCommand menuCommand)
        {
            if (FindPrefabContainer(out SnekUIPrefabContainer prefabContainer))
                SnekEditorPrefabUtility.InstantiatePrefabByReference(prefabContainer.Switch, menuCommand, true);
        }

        private static bool FindPrefabContainer(out SnekUIPrefabContainer prefabContainer)
        {
            prefabContainer = SnekScriptableObjectManager.GetAsset<SnekUIPrefabContainer>();

            if (!prefabContainer)
                Debug.LogError("Failed find prefab container, cannot create prefab.");

            return prefabContainer != null;
        }
    }
}