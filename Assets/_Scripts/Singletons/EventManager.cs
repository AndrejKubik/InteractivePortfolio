using System;
using Snek.SingletonManager;
using Snek.Utilities;

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
}
