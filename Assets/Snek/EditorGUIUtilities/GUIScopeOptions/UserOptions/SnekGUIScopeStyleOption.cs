using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekGUIScopeStyleOption : SnekGUIScopeOption
    {
        private readonly GUIStyle _style;

        public SnekGUIScopeStyleOption(GUIStyle style)
        {
            _style = style;
        }

        internal override void Apply(ref SnekGUIScopeOptions options)
        {
            options.Style = _style;
        }
    }
}
