using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SnekEditor.ScriptableObjectManager;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekReorderableList : ReorderableList
    {
        protected const float SelectionBorderWidth = 2f;

        protected const float DefaultElementLabelWidth = 80f;
        protected const float DefaultElementHeight = 30f;
        protected const float DefaultHeaderHeight = 30f;
        protected const float DefaultFooterHeight = 30f;
        protected const float DefaultElementContentVerticalPadding = 5f;

        protected GUIStyle _sectionStyle;
        protected GUIStyle _headerStyle;
        protected GUIStyle _labelStyle;
        protected GUIStyle _alertStyle;
        protected GUIStyle _buttonStyle;

        protected SnekInspectorGUILayoutSettings _visualSettings;
        protected SnekEditorIconsContainer _iconsContainer;

        protected readonly bool _isAddAllowed = true;
        protected readonly bool _isRemoveAllowed = true;
        protected readonly bool _isUsingCustomPropertyDrawer = false;

        public SnekReorderableList(
            SerializedObject serializedObject,
            SerializedProperty elements,
            bool allowAdd = true,
            bool allowRemove = true,
            bool allowMultiSelect = true,
            bool useCustomPropertyDrawer = false) : base(serializedObject, elements)
        {
            //get rid of default GUI
            showDefaultBackground = false;
            displayAdd = false;
            displayRemove = false;

            _isAddAllowed = allowAdd;
            _isRemoveAllowed = allowRemove;

            multiSelect = allowMultiSelect;

            _isUsingCustomPropertyDrawer = useCustomPropertyDrawer;

            headerHeight = 0f;
            footerHeight = GetFooterHeight();

            elementHeightCallback = GetElementHeight;
            drawElementBackgroundCallback = DrawElementBackground;
            drawElementCallback = DrawElement;
            drawFooterCallback = DrawFooter;

            onReorderCallbackWithDetails = OnReorderElements;

            _visualSettings = SnekScriptableObjectManager.GetAsset<SnekInspectorGUILayoutSettings>();
            _iconsContainer = SnekScriptableObjectManager.GetAsset<SnekEditorIconsContainer>();
        }



        protected ReadOnlyCollection<int> GetSelectedElements()
        {
            return selectedIndices;
        }

        private Color GetElementColor(int elementIndex)
        {
            List<Color> elementColors = _visualSettings.GetListElementColors();

            int colorIndex = elementIndex % elementColors.Count;

            return elementColors[colorIndex];
        }

        protected virtual float GetElementVerticalPadding()
        {
            return DefaultElementContentVerticalPadding;
        }

        protected virtual string GetHeaderLabel()
        {
            return serializedProperty.displayName;
        }

        protected virtual float GetFooterHeight()
        {
            return DefaultFooterHeight;
        }

        protected virtual float GetHeaderHeight()
        {
            return DefaultHeaderHeight;
        }

        protected virtual float GetElementHeightDefault()
        {
            return DefaultElementHeight;
        }

        protected virtual float GetElementHeightExpanded(SerializedProperty property)
        {
            float elementContentHeight = EditorGUI.GetPropertyHeight(property, true);

            if (IsElementLabelAllowed())
                elementContentHeight += GetElementHeightDefault() + GetElementVerticalPadding();

            if (SnekEditorUtility.IsPropertyCustomSerializable(property) && !_isUsingCustomPropertyDrawer)
                elementContentHeight -= EditorGUIUtility.singleLineHeight;

            return elementContentHeight;
        }

        private float GetElementHeight(int index)
        {
            SerializedProperty property = serializedProperty.GetArrayElementAtIndex(index);

            float contentHeight;

            if (property.hasVisibleChildren && property.isExpanded)
                contentHeight = GetElementHeightExpanded(property);
            else
                contentHeight = GetElementHeightDefault();

            return contentHeight + 2f * GetElementVerticalPadding();
        }


        /// <summary>
        /// <c>Index</c> allows different label values between elements, in case you want them to be uniform just ignore the parameter
        /// <list type="bullet">In case you want them to be uniform just ignore the parameter</list>
        /// </summary>
        protected virtual string GetElementLabel(int index)
        {
            return GetBaseElementLabel(index);
        }

        protected string GetBaseElementLabel(int index)
        {
            return $"Element {index}";
        }

        /// <summary>
        /// <c>Index</c> allows different width values between elements
        /// <list type="bullet">In case you want them to be uniform just ignore the parameter</list>
        /// </summary>
        protected virtual float GetElementLabelWidth(int index)
        {
            return DefaultElementLabelWidth;
        }



        protected bool IsElementSelected(int elementIndex)
        {
            return GetSelectedElements().Contains(elementIndex);
        }

        protected bool IsSelectionEmpty()
        {
            return GetSelectedElements().Count < 1;
        }

        protected virtual bool IsElementLabelAllowed()
        {
            return true;
        }

        protected virtual bool IsElementPropertyInteractionAllowed()
        {
            return true;
        }

        private bool Initialize()
        {
            if (!InitializeVisualSettings())
            {
                GUILayout.Label("Failed to find SnekInspectorGUILayoutSettings asset, cannot draw list.");

                return false;
            }

            if (!InitializeIconsContainer())
            {
                GUILayout.Label("Failed to find SnekEditorIconsContainer asset, cannot draw list.");

                return false;
            }

            if (!InitializeIcons())
            {
                GUILayout.Label("Failed to find required textures, cannot draw list.");

                return false;
            }

            InitializeStyles();

            return true;
        }

        private bool InitializeVisualSettings()
        {
            if (_visualSettings == null)
                return false;

            List<Color> elementColors = _visualSettings.GetListElementColors();

            return elementColors != null && elementColors.Count > 0;
        }

        private bool InitializeIconsContainer()
        {
            return _iconsContainer != null;
        }

        private bool InitializeIcons()
        {
            return _iconsContainer.GetFoldout(true) != null
                && _iconsContainer.GetFoldout(false) != null;
        }

        private void InitializeStyles()
        {
            if (_sectionStyle == null)
                _sectionStyle = SnekGUIStyles.SectionScope(_visualSettings.GetSectionPadding());

            if (_headerStyle == null)
                _headerStyle = SnekGUIStyles.BoldTextButton(12, true, true);

            if (_labelStyle == null)
                _labelStyle = SnekGUIStyles.Label(stretchWidth: true, stretchHeight: true);

            if (_alertStyle == null)
                _alertStyle = SnekGUIStyles.HelpBox();

            if (_buttonStyle == null)
                _buttonStyle = SnekGUIStyles.TextButton();
        }



        public virtual void Draw()
        {
            if (!Initialize())
                return;

            using (new SnekGUISectionScope(GUIContent.none, scopeStyle: _sectionStyle))
            {
                DrawHeader();

                if (serializedProperty.isExpanded)
                    DoLayoutList();
            }

            if (!IsSelectionEmpty() && !HasKeyboardControl())
            {
                ClearSelection();
                OnDeselectElements();
            }
        }

        protected virtual void DrawHeader()
        {
            Texture2D foldoutTexture = serializedProperty.isExpanded ?
                _iconsContainer.GetFoldout(true) : _iconsContainer.GetFoldout(false);

            bool buttonClicked = GUILayout.Button(
                GetHeaderLabel(),
                _headerStyle,
                GUILayout.Height(GetHeaderHeight()));

            if (buttonClicked)
                serializedProperty.isExpanded = !serializedProperty.isExpanded;

            Rect buttonRect = GUILayoutUtility.GetLastRect();

            var splitter = new SnekRectSplitter(buttonRect);
            Rect foldoutIconRect = splitter.TakeRight(buttonRect.height);

            SnekGUILayout.DrawTexture(foldoutIconRect, foldoutTexture);
        }

        protected virtual void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index == -1)
                return;

            EditorGUI.DrawRect(rect, GetElementColor(index));

            if (IsElementSelected(index))
                SnekGUILayout.DrawColoredBorder(rect, _visualSettings.GetBorderColor(), SelectionBorderWidth);
        }

        protected virtual void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = serializedProperty.GetArrayElementAtIndex(index);

            DrawElementProperty(rect, index, isActive, isFocused, property);
        }

        protected virtual void DrawElementProperty(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            var contentRect = new Rect(rect)
            {
                height = rect.height - 2 * GetElementVerticalPadding(),
                center = rect.center
            };

            using (new EditorGUI.PropertyScope(rect, GUIContent.none, property))
                if (!property.hasVisibleChildren)
                    DrawPrimitiveElementProperty(index, property, contentRect);
                else
                    DrawComplexElementProperty(index, property, contentRect);
        }

        private void DrawPrimitiveElementProperty(int index, SerializedProperty property, Rect contentRect)
        {
            if (!IsElementLabelAllowed())
            {
                using (new EditorGUI.DisabledScope(!IsElementPropertyInteractionAllowed()))
                    EditorGUI.PropertyField(contentRect, property, GUIContent.none);

                return;
            }

            var labelRect = new Rect(contentRect)
            {
                width = GetElementLabelWidth(index)
            };

            DrawElementLabel(index, labelRect);

            using (new EditorGUI.DisabledScope(!IsElementPropertyInteractionAllowed()))
                DrawElementPropertyField(contentRect, labelRect, index, property, out _);
        }

        private void DrawComplexElementProperty(int index, SerializedProperty property, Rect contentRect)
        {
            if (!IsElementLabelAllowed())
            {
                using (new EditorGUI.DisabledScope(!IsElementPropertyInteractionAllowed()))
                    if (SnekEditorUtility.IsPropertyCustomSerializable(property) && !_isUsingCustomPropertyDrawer)
                        SnekGUILayout.DrawInlineProperty(contentRect, property);
                    else
                        EditorGUI.PropertyField(contentRect, property, GUIContent.none, true);

                return;
            }

            var labelRect = new Rect(contentRect);

            if (property.isExpanded)
                labelRect.height = GetElementHeightDefault();

            DrawElementFoldoutButton(index, property, labelRect);

            using (new EditorGUI.DisabledScope(!IsElementPropertyInteractionAllowed()))
                DrawElementPropertyField(contentRect, labelRect, index, property, out _);
        }

        protected virtual void DrawElementLabel(int index, Rect labelRect)
        {
            EditorGUI.LabelField(labelRect, GetElementLabel(index), _labelStyle);
        }

        protected virtual void DrawElementFoldoutButton(int index, SerializedProperty property, Rect labelRect)
        {
            float foldoutIconSize = labelRect.height * 0.75f;

            var foldoutRect = new Rect(labelRect)
            {
                size = Vector2.one * foldoutIconSize,
                center = labelRect.center,
                x = labelRect.xMax - foldoutIconSize,
            };

            Texture2D foldoutIcon = _iconsContainer.GetFoldout(property.isExpanded);

            string label = IsElementLabelAllowed() ?
                GetElementLabel(index) : GetBaseElementLabel(index);

            bool isRightClicked =
                Event.current.type == EventType.MouseUp
                && labelRect.Contains(Event.current.mousePosition)
                && Event.current.button == 1;

            using (new EditorGUI.DisabledScope(isRightClicked))
            {
                bool buttonClicked = GUI.Button(labelRect, label, _buttonStyle);

                if (buttonClicked)
                    property.isExpanded = !property.isExpanded;

                SnekGUILayout.DrawTexture(foldoutRect, foldoutIcon);
            }
        }

        protected virtual void DrawElementPropertyField(Rect elementContentRect, Rect labelRect, int index, SerializedProperty property, out Rect propertyContentRect)
        {
            propertyContentRect = new Rect(elementContentRect);

            if (!property.hasVisibleChildren)
            {
                propertyContentRect.width = elementContentRect.width - labelRect.width;
                propertyContentRect.x = labelRect.xMax;

                using (new SnekOverrideTextFieldStyleScope())
                    EditorGUI.PropertyField(propertyContentRect, property, GUIContent.none);
            }
            else if (property.isExpanded)
            {
                propertyContentRect.height = elementContentRect.height - labelRect.height - 2f * GetElementVerticalPadding();
                propertyContentRect.y = labelRect.yMax + 1.5f * GetElementVerticalPadding(); //needs an extra half-padding to center the content due to already-applied element padding

                if (SnekEditorUtility.IsPropertyCustomSerializable(property) && !_isUsingCustomPropertyDrawer)
                    SnekGUILayout.DrawInlineProperty(propertyContentRect, property);
                else
                    EditorGUI.PropertyField(propertyContentRect, property, GUIContent.none, true);
            }
        }

        protected virtual void DrawFooter(Rect rect)
        {
            var rectSplitter = new SnekRectSplitter(rect);

            Rect addButtonRect = _isRemoveAllowed ?
                rectSplitter.TakeLeftPercentage(0.5f, true) : rect;

            Rect removeButtonRect = _isAddAllowed ?
                rectSplitter.TakeRightPercentage(0.5f, true) : rect;

            if (_isAddAllowed)
                DrawAddButton(addButtonRect);

            if (_isRemoveAllowed)
                using (new EditorGUI.DisabledScope(IsSelectionEmpty()))
                    DrawRemoveButton(removeButtonRect);
        }

        private void DrawAddButton(Rect rect)
        {
            bool buttonClicked = GUI.Button(rect, "Add", _buttonStyle);

            if (buttonClicked)
                OnAddButtonClick();
        }

        private void DrawRemoveButton(Rect rect)
        {
            bool buttonClicked = GUI.Button(rect, "Remove", _buttonStyle);

            if (buttonClicked)
                OnRemoveButtonClick();
        }



        private void OnAddButtonClick()
        {
            int newElementIndex = serializedProperty.arraySize;

            serializedProperty.InsertArrayElementAtIndex(newElementIndex);

            SerializedProperty newElement = serializedProperty.GetArrayElementAtIndex(newElementIndex);

            SnekEditorUtility.ResetPropertyValue(newElement);

            OnAddElement();
        }

        protected virtual void OnAddElement()
        {

        }

        private void OnRemoveButtonClick()
        {
            ReadOnlyCollection<int> selectedElements = GetSelectedElements();

            for (int i = selectedElements.Count - 1; i >= 0; i--)
                serializedProperty.DeleteArrayElementAtIndex(selectedElements[i]);

            ClearSelection();
            OnRemoveElement();
        }

        protected virtual void OnRemoveElement()
        {

        }

        protected virtual void OnReorderElements(ReorderableList list, int oldIndex, int newIndex)
        {

        }

        protected virtual void OnDeselectElements()
        {

        }
    }
}
