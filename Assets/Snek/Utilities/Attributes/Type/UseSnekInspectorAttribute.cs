using System;

namespace Snek.Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseSnekInspectorAttribute : Attribute
    {
        public bool IsInspectOnly = false;

        public UseSnekInspectorAttribute(bool isInspectOnly = false)
        {
            IsInspectOnly = isInspectOnly;
        }
    }
}