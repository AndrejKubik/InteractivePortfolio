using System.Collections.Generic;
using Snek.Utilities;
using SnekEditor.Utilities;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public abstract class SnekScriptableObjectInspectorCustom<T> : SnekScriptableObjectInspector where T : SnekScriptableObject
    {
        protected new IEnumerable<T> GetSelectedAssets()
        {
            return SnekEditorUtility.CastObjectsToType<T>(targets);
        }

        protected T GetSelectedAsset(int index = 0)
        {
            if (index < 0 || index >= targets.Length)
            {
                Debug.LogError("Trying to get selected asset with invalid index.");

                return null;
            }

            return targets[index] as T;
        }
    }
}