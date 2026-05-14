using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using SnekEditor.GUIUtilities;
using Snek.SingletonManager;

namespace SnekEditor.SingletonManager
{
    [CustomEditor(typeof(SnekSingletonManager))]
    public class SingletonManagerInspector : SnekMonoBehaviourInspectorCustom<SnekSingletonManager>
    {
        private SerializedProperty sp_SingletonPrefabs;
        protected override void OnCreateInspectorInstance()
        {
            sp_SingletonPrefabs = serializedObject.FindProperty(nameof(SnekSingletonManager.SingletonPrefabs));
        }

        protected override void OnPropertiesChange()
        {
            RemoveSingletonPrefabDuplicates();
        }

        protected override void DrawContent()
        {
            EditorGUILayout.PropertyField(sp_SingletonPrefabs);
        }

        private void RemoveSingletonPrefabDuplicates()
        {
            var noDuplicatesCollection = new HashSet<Object>();

            for (int i = sp_SingletonPrefabs.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty element = sp_SingletonPrefabs.GetArrayElementAtIndex(i);
                Object obj = element.objectReferenceValue;

                if (!noDuplicatesCollection.Add(obj))
                    sp_SingletonPrefabs.DeleteArrayElementAtIndex(i);
            }

            if (serializedObject.ApplyModifiedProperties())
                Debug.Log("Duplicate singleton prefabs detected and removed from the list.");
        }
    }
}