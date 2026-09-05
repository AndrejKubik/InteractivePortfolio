using System.Collections.Generic;
using Snek.EndlessCarousel;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

using OnVerticalDragEvent = Snek.EndlessCarousel.SnekEndlessCarousel.OnVerticalDragEvent;

[RequireComponent(typeof(SnekEndlessCarousel))]
[UseSnekInspector]
public class PortfolioCategoryCarouselController : SnekMonoBehaviour, ISnekInitializableExternal<PortfolioCategoryCarouselController.Data>
{
    public readonly struct Data
    {
        public readonly List<PortfolioProjectData> Projects;
        public readonly OnVerticalDragEvent OnVerticalDrag;
        public readonly PortfolioProjectButton ButtonPrefab;

        public Data(List<PortfolioProjectData> projects, OnVerticalDragEvent onVerticalDrag, PortfolioProjectButton buttonPrefab)
        {
            Projects = projects;
            OnVerticalDrag = onVerticalDrag;
            ButtonPrefab = buttonPrefab;
        }
    }

    private EventManager _eventManager;
    private PortfolioCategoryCarousel _projectButtonsCarousel;

    private List<PortfolioProjectData> _projects = new();
    private OnVerticalDragEvent _onVerticalDrag;
    private PortfolioProjectButton _buttonPrefab;

    public void OnBeforeInitialize(Data data)
    {
        _projects = data.Projects;
        _onVerticalDrag = data.OnVerticalDrag;
        _buttonPrefab = data.ButtonPrefab;
    }

    protected override void Initialize()
    {
        SnekSingletonManager.GetSingleton(out _eventManager);

        GetEssentialComponent(out _projectButtonsCarousel);
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_buttonPrefab, nameof(_buttonPrefab));

        ValidateEssentialComponent(_eventManager, nameof(_eventManager));

        if (_projects.Count < 1)
            FailValidation("No projects assigned.");

        if (_onVerticalDrag == null)
            FailValidation("Vertical drag callback not assigned.");

        base.Validate();
    }

    protected override void OnInitializationSuccess()
    {
        _projectButtonsCarousel.OnVerticalDrag += _onVerticalDrag;

        foreach (PortfolioProjectData project in _projects)
            CreateProjectButton(project);

        _projectButtonsCarousel.RunInitialization();
    }

    private void OnDestroy()
    {
        _projectButtonsCarousel.OnVerticalDrag -= _onVerticalDrag;
    }

    private void CreateProjectButton(PortfolioProjectData project)
    {
        PortfolioProjectButton button = Instantiate(_buttonPrefab, _projectButtonsCarousel.ElementContainer.transform);

        button.InitializeExternally(new PortfolioProjectButton.Data(project, _eventManager));
    }

    public bool IsDraggingCarousel()
    {
        return _projectButtonsCarousel.IsDragging;
    }
}
