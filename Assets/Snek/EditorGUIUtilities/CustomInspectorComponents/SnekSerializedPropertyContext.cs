using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekSerializedPropertyContext
    {
        public SerializedProperty SerializedProperty;

        private readonly Dictionary<string, SerializedProperty> _childProperties = new(StringComparer.Ordinal);

        public SnekSerializedPropertyContext(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public SerializedProperty GetChildProperty(string childPropertyName)
        {
            if (!_childProperties.TryGetValue(childPropertyName, out SerializedProperty childProperty))
            {
                childProperty = SerializedProperty.FindPropertyRelative(childPropertyName);

                _childProperties.Add(childPropertyName, childProperty);
            }

            return childProperty;
        }
    }
}
