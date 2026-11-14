using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectTagController : SnekMonoBehaviour
{
    [SerializeField] private PortfolioProjectButtonTag _tagPrefab;

    [Space(10f)]
    [SerializeField] private List<RectTransform> _tagContainers = new();

    private int _activeContainerIndex = 0;

    protected override void Validate()
    {
        ValidateEssentialComponent(_tagPrefab, nameof(_tagPrefab));

        if (_tagContainers == null || _tagContainers.Count < 1)
            FailValidation("No tag containers assigned.");
    }

    public void CreateTag(string tagText)
    {
        PortfolioProjectButtonTag newTag = Instantiate(_tagPrefab, GetActiveContainer());

        newTag.SetText(tagText);

        if (_tagContainers.Count > 1)
            SwitchActiveContainer();
    }

    public void ClearTags()
    {
        foreach (RectTransform tagContainer in _tagContainers)
            for (int i = tagContainer.childCount - 1; i >= 0; i--)
                Destroy(tagContainer.GetChild(i).gameObject);
    }

    private RectTransform GetActiveContainer()
    {
        return _tagContainers[_activeContainerIndex];
    }

    private void SwitchActiveContainer()
    {
        _activeContainerIndex++;

        if (_activeContainerIndex >= _tagContainers.Count)
            _activeContainerIndex = 0;
    }
}
