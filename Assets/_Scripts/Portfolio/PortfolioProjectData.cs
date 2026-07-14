using System;
using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
[CreateAssetMenu(fileName = "NewPortfolioProjectData", menuName = "Interactive Portfolio/Portfolio Project Data")]
public class PortfolioProjectData : SnekScriptableObject
{
    [SerializeField] private string _projectName = string.Empty;

    [Space(10f)]
    [SerializeField] private Sprite _thumbnail;
    [SerializeField] private string _videoDemoLink = string.Empty;

    [Space(10f)]
    [TextArea(1, 10)]
    [SerializeField] private string _descriptionText = string.Empty;

    [Space(10f)]
    [SerializeField] private List<DevelopmentHighlight> _developmentHighlights;

    [Serializable]
    private struct DevelopmentHighlight
    {
        [HideInInspector]
        public string Name; //prevents list element name override in the inspector

        [TextArea(1, 10)]
        public string Text;
    }

    public bool IsDataValid()
    {
        bool isDataValid = true;

        if (string.IsNullOrEmpty(_projectName))
            FailValidation("Project data name not assigned.", out isDataValid);

        if (_thumbnail == null)
            FailValidation("Project data thumbnail not assigned.", out isDataValid);

        //if (string.IsNullOrEmpty(_videoDemoLink))
        //    FailValidation("Project data video demo link not assigned.", out isDataValid);

        if (string.IsNullOrEmpty(_descriptionText))
            FailValidation("Project data description text not assigned.", out isDataValid);

        if (!IsEveryDevelopmentHighlightValid())
            FailValidation("Empty development highlights found inside project data.", out isDataValid);

        return isDataValid;

    }

    private bool IsEveryDevelopmentHighlightValid()
    {
        foreach (DevelopmentHighlight highlight in _developmentHighlights)
            if (string.IsNullOrEmpty(highlight.Text))
                return false;

        return true;
    }

    private void FailValidation(string message, out bool isDataValid)
    {
        Debug.LogError(message);

        isDataValid = false;
    }

    public string GetProjectName()
    {
        return _projectName;
    }

    public Sprite GetThumbnail()
    {
        return _thumbnail;
    }

    public string GetVideoDemoLink()
    {
        return _videoDemoLink;
    }

    public string GetDescriptionText()
    {
        return _descriptionText;
    }

    public string GetDevelopmentHighlights()
    {
        string finalText = string.Empty;

        foreach (DevelopmentHighlight highlight in _developmentHighlights)
        {
            if (!string.IsNullOrEmpty(finalText))
                finalText += "\n\n";

            finalText += $"- {highlight.Text}";
        }    
        
        return finalText;
    }
}
