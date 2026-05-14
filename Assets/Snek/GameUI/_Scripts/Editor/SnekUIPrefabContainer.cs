using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.GameUI
{
    [SnekAutoGenerateAsset("Assets/Snek/GameUI", nameof(SnekUIPrefabContainer))]
    [UseSnekInspector]
    public class SnekUIPrefabContainer : SnekScriptableObject
    {
        public GameObject Button;
        public GameObject Slider;
        public GameObject Switch;
    }
}