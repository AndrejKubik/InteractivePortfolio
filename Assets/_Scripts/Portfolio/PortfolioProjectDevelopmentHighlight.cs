using System;
using UnityEngine;

[Serializable]
public struct PortfolioProjectDevelopmentHighlight
{
    [HideInInspector]
    public string Name; //prevents list element name override in the inspector

    [TextArea(1, 10)]
    public string Text;
}
