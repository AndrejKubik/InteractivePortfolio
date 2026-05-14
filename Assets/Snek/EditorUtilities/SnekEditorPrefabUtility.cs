using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SnekEditor.Utilities
{
    public static class SnekEditorPrefabUtility
    {
        public static void InstantiatePrefabByPath(string assetPath, MenuCommand menuCommand, bool needsCanvas = false)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (!prefab)
            {
                Debug.LogError("Failed to find prefab at requested path, cannot create object.");

                return;
            }

            InstantiatePrefabByReference(prefab, menuCommand, needsCanvas);
        }

        public static void InstantiatePrefabByReference(GameObject prefab, MenuCommand menuCommand, bool needsCanvas = false)
        {
            if (!prefab)
            {
                Debug.LogError("Requested prefab is Null, cannot create object.");

                return;
            }


            GameObject parent = menuCommand.context as GameObject;

            if (needsCanvas)
                parent = EnsureCanvasParent(parent);

            var prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (parent)
                GameObjectUtility.SetParentAndAlign(prefabInstance, parent);

            Undo.RegisterCreatedObjectUndo(prefabInstance, "Create " + prefabInstance.name);

            Selection.activeObject = prefabInstance;
        }

        private static GameObject EnsureCanvasParent(GameObject parent)
        {
            if (parent != null)
            {
                Canvas existingCanvas = parent.GetComponentInParent<Canvas>();

                if (existingCanvas)
                    return parent;
            }

            Canvas canvas = Object.FindFirstObjectByType<Canvas>(); // Try finding a canvas in scene

            if (!canvas)
                canvas = CreateCanvas();

            return canvas.gameObject;
        }

        private static Canvas CreateCanvas()
        {
            var canvasGO = new GameObject(
                "Canvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

            CreateEventSystem();

            return canvas;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>())
                return;

            var eventSystem = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule));

            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        }
    }
}