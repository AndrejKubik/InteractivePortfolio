using UnityEngine;

namespace Snek.Utilities
{
    /// <summary>
    /// Blocks value change through the inspector while allowing it through code.
    /// <list type="table"><listheader>Works for:</listheader></list>
    /// <list type="bullet"><c>public</c></list>
    /// <list type="bullet"><c>protected</c></list>
    /// <list type="bullet"><c>[SerializeField] private</c></list>
    /// </summary>
    public class SnekInspectOnlyAttribute : PropertyAttribute
    {

    }
}