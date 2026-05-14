using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace SnekEditor.Utilities
{
    public static class SnekUnityEditorUtility
    {
        public static bool IsDragAndDropActive()
        {
            return DragAndDrop.objectReferences != null
                && DragAndDrop.objectReferences.Length > 0;
        }

        public static void ToggleInspectorWindowLock()
        {
            Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            Object[] inspectors = Resources.FindObjectsOfTypeAll(inspectorType);

            PropertyInfo isLockedProperty = inspectorType.GetProperty(
                "isLocked",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (Object inspector in inspectors)
                if (isLockedProperty != null)
                {
                    var currentValue = (bool)isLockedProperty.GetValue(inspector);

                    isLockedProperty.SetValue(inspector, !currentValue);

                    if (inspector is EditorWindow editor)
                        editor.Repaint();
                }
        }
    }
}
