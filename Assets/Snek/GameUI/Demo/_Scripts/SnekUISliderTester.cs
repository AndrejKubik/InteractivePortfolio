using System;
using Snek.GameUI;
using Snek.Utilities;
using TMPro;
using UnityEngine;

[UseSnekInspector]
public class SnekUISliderTester : SnekUISlider
{
    [Space(10f)]
    [SerializeField] private TextMeshProUGUI _valueTextMesh;

    protected override void Validate()
    {
        base.Validate();

        if (!_valueTextMesh)
            FailValidation("Value text mesh is not assigned.");
    }

    protected override void OnInitializationSuccess()
    {
        base.OnInitializationSuccess();

        UpdateValueText();
    }

    protected override void OnSliderMove(float newValue)
    {
        UpdateValueText();
    }

    protected override void OnHandleGrab()
    {
        Debug.Log("Handle grabbed");
    }

    protected override void OnHandleRelease()
    {
        Debug.Log("Handle released");
    }

    protected override void OnDragThresholdReach()
    {

    }

    protected override void OnDragAreaChange()
    {

    }

    private void UpdateValueText()
    {
        float textValue = MathF.Round(Slider.value, 2);

        _valueTextMesh.SetText(textValue.ToString());
    }
}
