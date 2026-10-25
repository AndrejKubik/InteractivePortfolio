using System;
using Snek.GameUI;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(SnekUIPointerInputDispatcher))]
[UseSnekInspector]
public class AllProjectsMenu : SnekMonoBehaviour
{
    private ScrollRect _scrollRect;
    private SnekUIPointerInputDispatcher _pointerInputDispatcher;

    private PortfolioCategory[] _portfolioCategories = null;

    protected override bool IsInitializedInStart() // initialized in start to ensure correct UI layout calculations
    {
        return true;
    }

    protected override void OnInitialize()
    {
        GetEssentialComponent(out _scrollRect);
        GetEssentialComponent(out _pointerInputDispatcher);

        _portfolioCategories = GetComponentsInChildren<PortfolioCategory>(true);
    }

    protected override void Validate()
    {
        if (_portfolioCategories == null || _portfolioCategories.Length < 1)
            FailValidation("No portfolio categories found.");
    }

    protected override void OnInitializationSuccess()
    {
        foreach (PortfolioCategory category in _portfolioCategories)
            category.Initialize(new PortfolioCategory.Data(_scrollRect, _pointerInputDispatcher));
    }
}
