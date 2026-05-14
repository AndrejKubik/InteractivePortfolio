using Snek.Utilities;
using UnityEngine;
using Snek.GameUI;

[UseSnekInspector]
public class SnekUIButtonTester : SnekUIButton
{
    [SerializeField] private string _message;

    protected override void OnButtonClick()
    {
        Debug.Log(_message);
    }
}
