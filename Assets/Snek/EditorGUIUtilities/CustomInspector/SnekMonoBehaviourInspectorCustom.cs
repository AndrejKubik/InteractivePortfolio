using System.Collections.Generic;
using Snek.Utilities;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public abstract class SnekMonoBehaviourInspectorCustom<T> : SnekMonoBehaviourInspector where T : SnekMonoBehaviour
    {
        /// <summary>
        /// Use to go over entire selection with some logic
        /// </summary>
        protected new IEnumerable<T> GetSelectedComponents()
        {
            return SnekEditorUtility.CastObjectsToType<T>(targets);
        }

        /// <summary>
        /// First selected component by default
        /// </summary>
        protected T GetSelectedComponent(int index = 0)
        {
            if(index < 0 || index >= targets.Length)
            {
                Debug.LogError("Trying to get selected component with invalid index.");

                return null;
            }

            return targets[index] as T;
        }
    }
}