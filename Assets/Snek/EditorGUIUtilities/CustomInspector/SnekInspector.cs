using System.Collections.Generic;
using Snek.Utilities;
using SnekEditor.ScriptableObjectManager;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    [CanEditMultipleObjects]
    public class SnekInspector : Editor
    {
        protected const string ScriptPropertyName = "m_Script";

        protected HashSet<string> _hiddenPropertyNames;
        protected UseSnekInspectorAttribute _attributeData;

        protected SnekInspectorGUILayoutSettings _layoutSettings;

        private GUIStyle _titleStyle;
        protected GUIStyle _sectionStyle;
        protected GUIStyle _sectionHeaderStyle;

        protected bool _isMouseOverComponent { get; private set; }
        protected Rect _inspectorRect { get; private set; }

        private void OnEnable()
        {
            _hiddenPropertyNames = new HashSet<string> { ScriptPropertyName };

            OnCreateInspectorInstance();
        }

        /// <summary>
        /// Called in OnEnable(), use for hooking SerializedProperty references and alike
        /// </summary>
        protected virtual void OnCreateInspectorInstance()
        {

        }

        public override void OnInspectorGUI()
        {
            if (!IsComponentUsingSnekInspector())
            {
                EditorGUILayout.HelpBox(
                    "This component can use Snek Inspector GUI.\n" +
                    "To enable it, add [UseSnekInspector] attribute to the class",
                    MessageType.Info);

                GUILayout.Space(10f);

                base.OnInspectorGUI();

                return;
            }

            if (!Initialize())
            {
                EditorGUILayout.HelpBox(
                    "Failed to find required components for Snek Inspector GUI.\n" +
                    "Drawing default Inspector instead.",
                    MessageType.Error);

                GUILayout.Space(10f);

                base.OnInspectorGUI();

                return;
            }

            serializedObject.Update();

            using (var componentScope = new SnekGUIVerticalScope())
            {
                Rect backgroundRect = GetGUIBackgroundRect(componentScope.GetRect());

                SnekGUILayout.DrawRect(backgroundRect, GetBackgroundColor());
                SnekGUILayout.DrawColoredBorder(backgroundRect, GetBorderColor(), GetBorderWidth());

                using (new SnekGUIVerticalScope(SnekGUIScopeAnchor.NoAnchor, SnekGUIScopeOption.SetPadding(_layoutSettings.GetComponentPadding())))
                {
                    using (new SnekGUIColoredScope(GetContentColor(), !EditorApplication.isPlaying))
                    {
                        DrawTitle();

                        GUILayout.Space(15f);

                        using (new EditorGUI.DisabledScope(_attributeData.IsInspectOnly))
                            DrawContent();
                    }
                }

                if (componentScope.GetRect() != default)
                {
                    _inspectorRect = backgroundRect;
                    _isMouseOverComponent = SnekGUIUtility.IsCursorOverRect(backgroundRect);
                }
            }

            if (serializedObject.ApplyModifiedProperties())
                OnPropertiesChange();
        }

        /// <summary>
        /// Called in <c>OnInspectorGUI()</c> before <c>DrawContent()</c>
        /// </summary>
        /// <returns><c>True</c> = Success, <c>False</c> = Fail</returns>
        protected virtual bool Initialize()
        {
            return InitializeLayoutSettings()
                && InitializeTitleStyle()
                && InitializeSectionStyle()
                && InitializeSectionHeaderStyle();
        }

        private bool InitializeTitleStyle()
        {
            if (_titleStyle == null)
                _titleStyle = SnekGUIStyles.BoldLabel(16, wordWrap: true);

            return _titleStyle != null;
        }

        private bool InitializeSectionStyle()
        {
            if (_sectionStyle == null)
                _sectionStyle = SnekGUIStyles.SectionScope(_layoutSettings.GetSectionPadding());

            return _sectionStyle != null;
        }

        private bool InitializeSectionHeaderStyle()
        {
            if (_sectionHeaderStyle == null)
                _sectionHeaderStyle = SnekGUIStyles.BoldLabel(14);

            return _sectionHeaderStyle != null;
        }

        private bool InitializeLayoutSettings()
        {
            if (_layoutSettings == null)
                _layoutSettings = SnekScriptableObjectManager.GetAsset<SnekInspectorGUILayoutSettings>();

            return _layoutSettings != null;
        }

        private bool IsComponentUsingSnekInspector()
        {
            return SnekEditorUtility.IsAttributeAssignedToType(target.GetType(), out _attributeData);
        }

        private void DrawTitle()
        {
            using (new SnekGUIHorizontalScope(SnekGUIScopeAnchor.Center))
                GUILayout.Label(ObjectNames.NicifyVariableName(GetTitleText()), _titleStyle);
        }

        /// <summary>
        /// Called in OnInspectorGUI(), use for new values validation logic
        /// </summary>
        protected virtual void OnPropertiesChange()
        {

        }

        private string GetTitleText()
        {
            return target.GetType().Name;
        }

        protected virtual Color GetBorderColor()
        {
            return _layoutSettings.GetBorderColor();
        }

        protected virtual float GetBorderWidth()
        {
            return _layoutSettings.GetBorderWidth();
        }

        protected virtual Color GetBackgroundColor()
        {
            return _layoutSettings.GetBackgroundColor();
        }

        protected virtual Color GetContentColor()
        {
            return _layoutSettings.GetContentColor();
        }

        private Rect GetGUIBackgroundRect(Rect componentScopeRect)
        {
            Vector2 positionOffset = _layoutSettings.GetBackgroundPositionOffset();
            Vector2 sizeOffset = _layoutSettings.GetBackgroundSizeOffset();

            Rect backgroundRect = new Rect(componentScopeRect);
            backgroundRect.x += positionOffset.x;
            backgroundRect.y += positionOffset.y;
            backgroundRect.width += sizeOffset.x;
            backgroundRect.height += sizeOffset.y;

            return backgroundRect;
        }

        /// <summary>
        /// Called in OnInspectorGUI(), use as main container for all drawing logic.
        /// </summary>
        protected virtual void DrawContent()
        {
            DrawBaseProperties();
        }

        /// <summary>
        /// Draws all base properties which are not hidden, like in a regular inspector
        /// </summary>
        protected void DrawBaseProperties()
        {
            GUIStyle style = IsAnyPropertySerialized() ? _sectionStyle : GUIStyle.none;

            using (new SnekGUISectionScope(GUIContent.none, scopeStyle: style))
                DrawPropertiesExcluding(serializedObject, GetHiddenPropertyNames());
        }

        private bool IsAnyPropertySerialized()
        {
            SerializedProperty prop = serializedObject.GetIterator();

            return prop.NextVisible(true) && prop.NextVisible(false);
        }

        protected string[] GetHiddenPropertyNames()
        {
            string[] array = new string[_hiddenPropertyNames.Count];

            _hiddenPropertyNames.CopyTo(array);

            return array;
        }

        protected void HideProperty(SerializedProperty property)
        {
            if (_hiddenPropertyNames.Add(property.name))
                Repaint();
        }

        protected void ShowPropety(SerializedProperty property)
        {
            if (_hiddenPropertyNames.Remove(property.name))
                Repaint();
        }

        protected SerializedProperty GetProperty(string propertyName, bool hideFromBaseProperties = false)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                Debug.LogError($"Failed to find property: {propertyName}");

                return null;
            }

            if (hideFromBaseProperties)
                HideProperty(property);

            return property;
        }
    }
}
