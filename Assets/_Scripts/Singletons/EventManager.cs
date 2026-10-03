using System;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine.InputSystem;

[UseSnekInspector]
public class EventManager : SnekMonoSingleton
{
    public delegate void RequestProjectOverviewEvent(PortfolioProject project);
    public event RequestProjectOverviewEvent OnRequestProjectOverview;
    public void RequestProjectOverview(PortfolioProject project)
    {
        OnRequestProjectOverview?.Invoke(project);
    }

    public event Action OnRequestShowAllProjects;
    public void RequestShowAllProjects()
    {
        OnRequestShowAllProjects?.Invoke();
    }

    public event Action OnPressEscapeKey;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnPressEscapeKey?.Invoke();
    }
}
