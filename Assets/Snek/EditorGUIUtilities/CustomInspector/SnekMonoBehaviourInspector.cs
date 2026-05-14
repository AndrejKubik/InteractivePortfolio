using System.Collections.Generic;
using Snek.Utilities;
using SnekEditor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    [CustomEditor(typeof(SnekMonoBehaviour), true), CanEditMultipleObjects]
    public class SnekMonoBehaviourInspector : SnekInspector
    {
        private bool _isAnySelectedComponentEnabled = true;

        /// <summary>
        /// Use to go over entire selection with some logic
        /// </summary>
        protected IEnumerable<SnekMonoBehaviour> GetSelectedComponents()
        {
            return SnekEditorUtility.CastObjectsToType<SnekMonoBehaviour>(targets);
        }
        
        public sealed override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_isAnySelectedComponentEnabled != IsAnySelectedComponentEnabled())
                OnComponentToggle(IsAnySelectedComponentEnabled());
        }

        protected virtual void OnComponentToggle(bool newState)
        {
            _isAnySelectedComponentEnabled = IsAnySelectedComponentEnabled();
            
            if (newState == true)
                OnPropertiesChange();
        }

        protected bool IsAnySelectedComponentEnabled()
        {
            foreach (SnekMonoBehaviour component in GetSelectedComponents())
                if (component.enabled)
                    return true;

            return false;
        }

        protected override Color GetBorderColor()
        {
            return IsAnySelectedComponentEnabled() ? 
                _layoutSettings.GetBorderColor() : _layoutSettings.GetDisabledBorderColor();
        }

        protected override Color GetContentColor()
        {
            return IsAnySelectedComponentEnabled() ? 
                _layoutSettings.GetContentColor() : _layoutSettings.GetDisabledContentColor();
        }
    }
}
