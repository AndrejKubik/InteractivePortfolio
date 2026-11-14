using System;
using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectDevelopmentStateTag : PortfolioProjectButtonTag
{
    [Serializable]
    private struct DevelopmentStateTagData
    {
        public PortfolioProjectDevelopmentState DevelopmentState;
        public Color Color;
        public string Text;
    }

    [SerializeField] private List<DevelopmentStateTagData> _developmentStateTagDataCombinations = new();

    protected override void Validate()
    {
        base.Validate();

        if (!IsEveryDevelopmentStatePopulatedWithTagData())
            FailValidation(
                $"{nameof(_developmentStateTagDataCombinations)} list is missing development states or has duplicates.\n" +
                $"Make sure every development state tag data has a unique development state value.");
        else if (!IsEveryDevelopmentStateTagDataValid())
            FailValidation("Some development state tag data has completely transparent color or empty text.");
    }

    private bool IsEveryDevelopmentStatePopulatedWithTagData()
    {
        Array developmentStateValues = Enum.GetValues(typeof(PortfolioProjectDevelopmentState));

        if (_developmentStateTagDataCombinations.Count != developmentStateValues.Length)
            return false;

        foreach (PortfolioProjectDevelopmentState state in developmentStateValues)
        {
            int foundElements = 0;

            foreach (DevelopmentStateTagData tagData in _developmentStateTagDataCombinations)
                if (tagData.DevelopmentState == state)
                    foundElements++;

            if (foundElements != 1)
                return false;
        }

        return true;
    }

    private bool IsEveryDevelopmentStateTagDataValid()
    {
        foreach (DevelopmentStateTagData tagData in _developmentStateTagDataCombinations)
            if (!IsDevelopmentStateTagDataValid(tagData))
                return false;

        return true;
    }

    private bool IsDevelopmentStateTagDataValid(DevelopmentStateTagData data)
    {
        return data.Color != default && !string.IsNullOrWhiteSpace(data.Text);
    }

    public void SetDevelopmentState(PortfolioProjectDevelopmentState newState)
    {
        if (!IsAnyDevelopmentStateTagDataWithState(newState, out int elementIndex))
        {
            Debug.LogError(
                $"No development state tag data found with state: {newState}\n" +
                $"Cannot update development state tag.");

            return;
        }

        DevelopmentStateTagData tagData = _developmentStateTagDataCombinations[elementIndex];

        SetColor(tagData.Color);
        SetText(tagData.Text);
    }

    private bool IsAnyDevelopmentStateTagDataWithState(PortfolioProjectDevelopmentState state, out int elementIndex)
    {
        elementIndex = _developmentStateTagDataCombinations.FindIndex(IsDevelopmentStateTagDataWithState);

        return elementIndex != -1;

        bool IsDevelopmentStateTagDataWithState(DevelopmentStateTagData tagData)
        {
            return tagData.DevelopmentState == state;
        }
    }
}
