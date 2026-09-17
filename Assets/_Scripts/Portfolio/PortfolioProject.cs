using System.Collections.Generic;
using UnityEngine;

public class PortfolioProject
{
    private const string ProxyDemoVideosBaseUrl = "https://interactive-portfolio.andrejkk97.workers.dev/";
    private const string DemoVideosBaseUrl = "https://github.com/AndrejKubik/InteractivePortfolio/releases/download/demo-videos/";

    private readonly string _projectName = string.Empty;
    private readonly Sprite _thumbnail;
    private readonly string _videoDemoUrl = string.Empty;
    private readonly string _descriptionText = string.Empty;
    private readonly List<PortfolioProjectDevelopmentHighlight> _developmentHighlights;

    public PortfolioProject(PortfolioProjectData data)
    {
        _projectName = data.ProjectName;
        _thumbnail = data.Thumbnail;
        _videoDemoUrl = data.VideoDemoUrl;
        _descriptionText = data.Description;
        _developmentHighlights = data.DevelopmentHighlights;
    }

    public bool IsDataValid()
    {
        bool isDataValid = true;

        if (string.IsNullOrEmpty(_projectName))
            FailValidation("Project data name not assigned.", out isDataValid);

        if (_thumbnail == null)
            FailValidation("Project data thumbnail not assigned.", out isDataValid);

        if (string.IsNullOrEmpty(_videoDemoUrl))
            FailValidation("Project data video demo link not assigned.", out isDataValid);
        else if (!_videoDemoUrl.StartsWith(DemoVideosBaseUrl))
            FailValidation("Project data video demo link is invalid.", out isDataValid);

        if (string.IsNullOrEmpty(_descriptionText))
            FailValidation("Project data description text not assigned.", out isDataValid);

        if (!IsEveryDevelopmentHighlightValid())
            FailValidation("Empty development highlights found inside project data.", out isDataValid);

        return isDataValid;

    }

    private bool IsEveryDevelopmentHighlightValid()
    {
        foreach (PortfolioProjectDevelopmentHighlight highlight in _developmentHighlights)
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

    public string GetVideoDemoUrl()
    {
        string fileName = _videoDemoUrl.Replace(DemoVideosBaseUrl, string.Empty);

        return ProxyDemoVideosBaseUrl + fileName;
    }

    public string GetDescriptionText()
    {
        return _descriptionText;
    }

    public string GetDevelopmentHighlights()
    {
        string finalText = string.Empty;

        foreach (PortfolioProjectDevelopmentHighlight highlight in _developmentHighlights)
        {
            if (!string.IsNullOrEmpty(finalText))
                finalText += "\n\n";

            finalText += $"- {highlight.Text}";
        }

        return finalText + "\n";
    }
}
