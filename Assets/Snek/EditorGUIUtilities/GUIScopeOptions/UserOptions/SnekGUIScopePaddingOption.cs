using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekGUIScopePaddingOption : SnekGUIScopeOption
    {
        private readonly RectOffset _padding;

        public SnekGUIScopePaddingOption(RectOffset padding)
        {
            _padding = padding;
        }

        internal override void Apply(ref SnekGUIScopeOptions options)
        {
            options.Padding = _padding;
        }
    }
}
