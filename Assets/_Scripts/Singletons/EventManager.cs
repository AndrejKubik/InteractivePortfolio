using System;
using Snek.SingletonManager;
using Snek.Utilities;

[UseSnekInspector]
public class EventManager : SnekMonoSingleton
{
    public delegate void RequestProjectOverviewEvent(PortfolioProjectData projectData);
    public event RequestProjectOverviewEvent OnRequestProjectOverview;
    public void RequestProjectOverview(PortfolioProjectData projectData)
    {
        OnRequestProjectOverview?.Invoke(projectData);
    }

    public event Action OnRequestShowAllProjects;
    public void RequestShowAllProjects()
    {
        OnRequestShowAllProjects?.Invoke();
    }
}
