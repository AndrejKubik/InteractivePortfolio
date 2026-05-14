using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.ScriptableObjectManager
{
    [UseSnekInspector(true)]
    public class SnekScriptableObjectManagerCache : SnekScriptableObject
    {
        [Space(10f)]
        public List<ScriptableObject> GeneratedAssets = new List<ScriptableObject>();
    }
}
