using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
[CreateAssetMenu(fileName = "NewPortfolioProjectData", menuName = "Interactive Portfolio/Portfolio Project Data")]
public class PortfolioProjectData : SnekScriptableObject
{
    public string ProjectName = string.Empty;

    [Space(10f)]
    public Sprite Thumbnail;

    [Space(10f)]
    [TextArea]
    public string VideoDemoUrl = string.Empty;

    [Space(10f)]
    [TextArea(1, 10)]
    public string Description = string.Empty;

    [Space(10f)]
    public List<PortfolioProjectDevelopmentHighlight> DevelopmentHighlights;
}
