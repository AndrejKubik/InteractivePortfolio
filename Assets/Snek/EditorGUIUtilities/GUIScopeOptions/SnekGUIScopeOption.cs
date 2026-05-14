using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public abstract class SnekGUIScopeOption
    {
        internal abstract void Apply(ref SnekGUIScopeOptions options);

        public static SnekGUIScopeOption SetStyle(GUIStyle style)
        {
            return new SnekGUIScopeStyleOption(style);
        }

        public static SnekGUIScopeOption SetPadding(RectOffset padding)
        {
            return new SnekGUIScopePaddingOption(padding);
        }

        public static SnekGUIScopeOption SetGUILayoutOptions(params GUILayoutOption[] options)
        {
            return new SnekGUIScopeGUILayoutOption(options);
        }
    }
}
