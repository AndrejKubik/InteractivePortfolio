using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SnekEditor.GUIUtilities
{
    /// <summary>
    /// When changing property values without using <c>SerializedProperty</c> approach, change them inside of this scope
    /// </summary>
    public class SnekDirectValueChangeScope : IDisposable
    {
        private readonly Object _propertyHolder;

        public SnekDirectValueChangeScope(Object propertyHolder)
        {
            _propertyHolder = propertyHolder;
        }

        public SnekDirectValueChangeScope(Object propertyHolder, string undoActionName)
        {
            _propertyHolder = propertyHolder;

            Undo.RecordObject(propertyHolder, undoActionName);
        }

        public void Dispose()
        {
            EditorUtility.SetDirty(_propertyHolder);
        }
    }
}