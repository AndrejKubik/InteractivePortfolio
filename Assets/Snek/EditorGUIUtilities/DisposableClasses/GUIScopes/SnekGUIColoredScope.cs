using System;
using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    public class SnekGUIColoredScope : IDisposable
    {
        private Color _currentContentColor;

        public SnekGUIColoredScope(Color color, bool condition = true)
        {
            _currentContentColor = GUI.color;

            if (condition == true)
                GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = _currentContentColor;
        }
    }
}