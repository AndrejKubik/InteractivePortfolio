using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekGUIScopeGUILayoutOption : SnekGUIScopeOption
    {
        private readonly GUILayoutOption[] _guiLayoutOptions;

        public SnekGUIScopeGUILayoutOption(params GUILayoutOption[] guiLayoutOptions)
        {
            _guiLayoutOptions = guiLayoutOptions;
        }

        internal override void Apply(ref SnekGUIScopeOptions options)
        {
            options.GUILayoutOptions = _guiLayoutOptions;
        }
    }
}
