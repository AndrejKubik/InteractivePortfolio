using System;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    /// <summary>
    /// GUI scope which draws everything inside it horizontally
    /// </summary>
    public class SnekGUIHorizontalScope : IDisposable
    {
        private SnekGUIScope _guiScope;

        public SnekGUIHorizontalScope(SnekGUIScopeAnchor anchor = SnekGUIScopeAnchor.NoAnchor, params SnekGUIScopeOption[] options)
        {
            _guiScope = new SnekGUIScope(SnekGUIDrawDirection.Horizontal, anchor, options);
        }

        public SnekGUIHorizontalScope(SnekGUIScopeAnchor? anchor, params GUILayoutOption[] options)
        {
            if (!anchor.HasValue)
                anchor = SnekGUIScopeAnchor.NoAnchor;

            _guiScope = new SnekGUIScope(
                SnekGUIDrawDirection.Horizontal,
                anchor.Value,
                SnekGUIScopeOption.SetGUILayoutOptions(options));
        }

        public SnekGUIHorizontalScope(params GUILayoutOption[] options)
        {
            _guiScope = new SnekGUIScope(
                SnekGUIDrawDirection.Horizontal,
                SnekGUIScopeAnchor.NoAnchor,
                SnekGUIScopeOption.SetGUILayoutOptions(options));
        }

        public void Dispose()
        {
            _guiScope.Dispose();
        }

        public Rect GetRect()
        {
            return _guiScope.Rect;
        }
    }
}
