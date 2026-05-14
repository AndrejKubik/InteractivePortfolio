using System;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekGUIScope : IDisposable
    {
        private readonly SnekGUIDrawDirection _drawDirection;
        private readonly SnekGUIScopeAnchor _anchor;
        private readonly SnekGUIScopeOptions _options;
        private readonly GUILayoutOption[] _layoutOptions;
        private readonly GUIStyle _finalStyle;

        public Rect Rect { get; private set; }

        public SnekGUIScope(SnekGUIDrawDirection drawDirection, SnekGUIScopeAnchor anchor, params SnekGUIScopeOption[] scopeOptions)
        {
            _drawDirection = drawDirection;
            _anchor = anchor;
            _options = new SnekGUIScopeOptions();

            foreach (SnekGUIScopeOption option in scopeOptions)
                option.Apply(ref _options);

            _finalStyle = _options.Style == null ? GUIStyle.none : _options.Style;

            if (_options.Padding != null)
                _finalStyle = SnekGUIUtility.GetStyleWithPadding(_finalStyle, _options.Padding);

            _layoutOptions = _options.GUILayoutOptions;

            BeginScope();
        }

        private void BeginScope()
        {
            if (_drawDirection == SnekGUIDrawDirection.Horizontal)
                Rect = EditorGUILayout.BeginHorizontal(_finalStyle, _layoutOptions);
            else if (_drawDirection == SnekGUIDrawDirection.Vertical)
                Rect = EditorGUILayout.BeginVertical(_finalStyle, _layoutOptions);

            if (_anchor == SnekGUIScopeAnchor.Center || _anchor == SnekGUIScopeAnchor.End)
                GUILayout.FlexibleSpace();
        }

        public virtual void Dispose()
        {
            if (_anchor == SnekGUIScopeAnchor.Start || _anchor == SnekGUIScopeAnchor.Center)
                GUILayout.FlexibleSpace();

            if (_drawDirection == SnekGUIDrawDirection.Horizontal)
                EditorGUILayout.EndHorizontal();
            else if (_drawDirection == SnekGUIDrawDirection.Vertical)
                EditorGUILayout.EndVertical();
        }
    }
}
