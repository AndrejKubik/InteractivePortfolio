using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace SnekEditor.Utilities
{
    public static class SnekEditorShortcuts
    {
        private const string ShortcutsRoot = "Snek/";

        [Shortcut(ShortcutsRoot + "Toggle Inspector Lock", KeyCode.Q, ShortcutModifiers.Alt)]
        public static void ToggleInspectorLock()
        {
            SnekUnityEditorUtility.ToggleInspectorWindowLock();
        }
    }
}
