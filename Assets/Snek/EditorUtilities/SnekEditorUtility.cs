using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SnekEditor.Utilities
{
    public static class SnekEditorUtility
    {
        public const string MenuItemRoot = "Snek Production/";

        public static bool IsAttributeAssignedToType<T>(Type targetType, out T attribute, bool checkParentTypes = false) where T : Attribute
        {
            attribute = targetType.GetCustomAttribute<T>(checkParentTypes);

            return attribute != null;
        }

        public static IEnumerable<T> CastObjectsToType<T>(Object[] targets) where T : Object
        {
            foreach (Object target in targets)
                yield return (T)target;
        }

        public static bool IsObjectInArray(Object target, SerializedProperty arrayProperty)
        {
            if (arrayProperty == null || !arrayProperty.isArray)
                return false;

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);

                if (element.propertyType != SerializedPropertyType.ObjectReference)
                {
                    Debug.LogError("Trying to look for an Object reference inside of a non-Object element type array, aborting...");

                    return false;
                }

                if (element.objectReferenceValue == target)
                    return true;
            }

            return false;
        }

        public static void ResetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:

                    property.objectReferenceValue = null;
                    break;

                case SerializedPropertyType.ManagedReference:

                    property.managedReferenceValue = null;
                    break;

                case SerializedPropertyType.String:

                    property.stringValue = string.Empty;
                    break;

                case SerializedPropertyType.Integer:

                    property.intValue = 0;
                    break;

                case SerializedPropertyType.Boolean:

                    property.boolValue = false;
                    break;

                case SerializedPropertyType.Float:

                    property.floatValue = 0f;
                    break;

                case SerializedPropertyType.Enum:

                    property.enumValueIndex = 0;
                    break;

                case SerializedPropertyType.Vector2:

                    property.vector2Value = Vector2.zero;
                    break;

                case SerializedPropertyType.Vector3:

                    property.vector3Value = Vector3.zero;
                    break;

                case SerializedPropertyType.Vector4:

                    property.vector4Value = Vector4.zero;
                    break;

                case SerializedPropertyType.Color:

                    property.colorValue = Color.white;
                    break;

                case SerializedPropertyType.Generic:

                    ResetGenericProperty(property);
                    break;
            }
        }

        public static void ResetGenericProperty(SerializedProperty property)
        {
            SerializedProperty copy = property.Copy();
            SerializedProperty end = copy.GetEndProperty();

            bool enterChildren = true;

            while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
            {
                enterChildren = false;

                ResetPropertyValue(copy);
            }
        }

        public static bool IsPropertyCustomSerializable(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Generic && !property.isArray;
        }
    }
}
