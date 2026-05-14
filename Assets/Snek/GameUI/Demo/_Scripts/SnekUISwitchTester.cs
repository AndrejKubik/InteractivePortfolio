using Snek.Utilities;
using UnityEngine;
using Snek.GameUI;

[UseSnekInspector]
public class SnekUISwitchTester : SnekUISwitch
{
    protected override void OnSwitchDisable()
    {
        Debug.Log("Switch is now OFF");
    }

    protected override void OnSwitchEnable()
    {
        Debug.Log("Switch is now ON");
    }
}
