using System;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class ScreenManager : SnekMonoSingleton
{
    private int _currentWidth = 0;
    private int _currentHeight = 0;

    public event Action OnScreenSizeChanged;

    protected override void OnInitialize()
    {
        _currentWidth = Screen.width;
        _currentHeight = Screen.height;
    }

    protected override void OnUpdate()
    {
        bool isSizeChanged = false;

        if (Screen.width != _currentWidth)
        {
            _currentWidth = Screen.width;

            isSizeChanged = true;
        }

        if (Screen.height != _currentHeight)
        {
            _currentHeight = Screen.height;

            isSizeChanged = true;
        }

        if (isSizeChanged)
        {
            OnScreenSizeChanged?.Invoke();
            
            Canvas.ForceUpdateCanvases();
        }
    }
}
