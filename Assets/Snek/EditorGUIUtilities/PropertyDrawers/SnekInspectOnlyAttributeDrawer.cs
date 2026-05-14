using Snek.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    [CustomPropertyDrawer(typeof(SnekInspectOnlyAttribute))]
    public class SnekInspectOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            using (new EditorGUI.DisabledScope(true))
                EditorGUI.PropertyField(position, property, true);

            EditorGUI.EndProperty();
        }
    }
}
