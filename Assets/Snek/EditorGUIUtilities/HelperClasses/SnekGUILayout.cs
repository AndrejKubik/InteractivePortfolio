using SnekEditor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public static class SnekGUILayout
    {
        public const float DefaultFieldWidth = 50f;
        public const float DefaultFieldHeight = 20f;
        public const float UnityCheckboxWidth = 16f;
        public const float UnityFoldoutArrowSize = 15f;

        public static void DrawEnumField(SerializedProperty property, string[] valueNames, params GUILayoutOption[] options)
        {
            using (var scope = new SnekPropertyFieldScope(property))
            {
                int newValue = EditorGUILayout.Popup(property.intValue, valueNames, options);

                if (scope.IsValueChanged())
                    property.intValue = newValue;
            }
        }

        public static void DrawBoolField(SerializedProperty property, params GUILayoutOption[] options)
        {
            using (var scope = new SnekPropertyFieldScope(property))
            {
                bool newValue = EditorGUILayout.Toggle(property.boolValue, options);

                if (scope.IsValueChanged())
                    property.boolValue = newValue;
            }
        }

        public static void DrawIntField(SerializedProperty property, GUIStyle style = null, params GUILayoutOption[] options)
        {
            using (var scope = new SnekPropertyFieldScope(property))
            {
                int newValue = EditorGUILayout.IntField(property.intValue, style, options);

                if (scope.IsValueChanged())
                    property.intValue = newValue;
            }
        }

        public static void DrawFloatField(SerializedProperty property, GUIStyle style = null, params GUILayoutOption[] options)
        {
            using (var scope = new SnekPropertyFieldScope(property))
            {
                float newValue = EditorGUILayout.FloatField(property.floatValue, style, options);

                if (scope.IsValueChanged())
                    property.floatValue = newValue;
            }
        }

        public static void DrawStringField(SerializedProperty property, GUIStyle style = null, params GUILayoutOption[] options)
        {
            using (var scope = new SnekPropertyFieldScope(property))
            {
                string newValue = EditorGUILayout.TextField(property.stringValue, style, options);

                if (scope.IsValueChanged())
                    property.stringValue = newValue;
            }
        }

        public static void DrawHorizontalSlider(SerializedProperty currentValueProperty, float minValue, float maxValue)
        {
            using (var scope = new SnekPropertyFieldScope(currentValueProperty))
            {
                float newValue = GUILayout.HorizontalSlider(currentValueProperty.floatValue, minValue, maxValue);

                if (scope.IsValueChanged())
                    currentValueProperty.floatValue = newValue;
            }
        }

        public static void DrawObjectField<T>(SerializedProperty property, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
        {
            using (var scope = new SnekPropertyFieldScope(property))
            {
                Object newValue = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(T), allowSceneObjects, options);

                if (scope.IsValueChanged())
                    property.objectReferenceValue = newValue;
            }
        }

        public static void DrawRectTransparent(Rect rect, Color color, float alpha = 0.25f)
        {
            color.a = alpha;

            DrawRect(rect, color);
        }

        public static void DrawRect(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawColoredBorder(Rect rect, Color borderColor, float borderWidth)
        {
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0, borderColor, borderWidth, 0f);
        }

        public static void DrawTexture(Rect rect, Texture texture, Color? color = null)
        {
            if (!color.HasValue)
                color = Color.white;

            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true, 0, color.Value, 0f, 0f);
        }

        public static void DrawTextureTransparent(Rect rect, Texture texture, Color? color = null, float alpha = 0.25f)
        {
            if (!color.HasValue)
                color = Color.white;

            Color transparentColor = color.Value;
            transparentColor.a = alpha;

            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true, 0, transparentColor, 0f, 0f);
        }

        public static void DrawAlertMessage(string message, GUIStyle textStyle = null, Color? color = null)
        {
            if (!color.HasValue)
                color = GUI.color;

            if (textStyle == null)
                textStyle = new GUIStyle(EditorStyles.whiteBoldLabel)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    wordWrap = true
                };

            using (var scope = new SnekGUIVerticalScope(SnekGUIScopeAnchor.Center))
            {
                DrawRectTransparent(scope.GetRect(), Color.white, 0.1f);

                GUILayout.Label(message, textStyle);

                DrawColoredBorder(scope.GetRect(), color.Value, 1f);
            }
        }

        public static void DrawInlineProperty(Rect position, SerializedProperty property)
        {
            property.isExpanded = true;

            using(new EditorGUI.PropertyScope(position, GUIContent.none, property))
            {
                float y = position.y;

                SerializedProperty copy = property.Copy();
                SerializedProperty end = copy.GetEndProperty();

                bool enterChildren = true;

                while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
                {
                    float height = EditorGUI.GetPropertyHeight(copy, true);
                    Rect fieldRect = new Rect(position.x, y, position.width, height);

                    EditorGUI.PropertyField(fieldRect, copy, true);

                    y += height + EditorGUIUtility.standardVerticalSpacing;
                    enterChildren = false;
                }
            }
        }
    }
}
