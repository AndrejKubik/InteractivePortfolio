using System.Collections.Generic;
using Snek.Utilities;
using SnekEditor.Utilities;
using UnityEditor;

namespace SnekEditor.GUIUtilities
{
    [CustomEditor(typeof(SnekScriptableObject), true), CanEditMultipleObjects]
    public class SnekScriptableObjectInspector : SnekInspector
    {
        protected IEnumerable<SnekScriptableObject> GetSelectedAssets()
        {
            return SnekEditorUtility.CastObjectsToType<SnekScriptableObject>(targets);
        }

        public sealed override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}