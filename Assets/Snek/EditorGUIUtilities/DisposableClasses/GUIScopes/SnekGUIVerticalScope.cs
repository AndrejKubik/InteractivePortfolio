using System;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    /// <summary>
    /// GUI scope which draws everything inside it vertically
    /// </summary>
    public class SnekGUIVerticalScope : IDisposable
    {
        private SnekGUIScope _guiScope;

        public Rect GetRect()
        {
            return _guiScope.Rect;
        }

        public SnekGUIVerticalScope(SnekGUIScopeAnchor anchor = SnekGUIScopeAnchor.NoAnchor, params SnekGUIScopeOption[] options)
        {
            _guiScope = new SnekGUIScope(SnekGUIDrawDirection.Vertical, anchor, options);
        }

        public SnekGUIVerticalScope(SnekGUIScopeAnchor? anchor, params GUILayoutOption[] options)
        {
            if (!anchor.HasValue)
                anchor = SnekGUIScopeAnchor.NoAnchor;

            _guiScope = new SnekGUIScope(
                SnekGUIDrawDirection.Vertical,
                anchor.Value,
                SnekGUIScopeOption.SetGUILayoutOptions(options));
        }

        public SnekGUIVerticalScope(params GUILayoutOption[] options)
        {
            _guiScope = new SnekGUIScope(
                SnekGUIDrawDirection.Vertical,
                SnekGUIScopeAnchor.NoAnchor,
                SnekGUIScopeOption.SetGUILayoutOptions(options));
        }

        public void Dispose()
        {
            _guiScope.Dispose();
        }
    }
}
