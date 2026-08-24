using Snek.Utilities;
using UnityEngine;

namespace Snek.GameUIPlus
{
    [UseSnekInspector]
    public class SnekUIButtonWithSFXSimple : SnekUIButtonWithSFX
    {
        [SerializeField] private AudioClip _buttonSFX;

        protected override void Validate()
        {
            base.Validate();

            if (!_buttonSFX)
                FailValidation("Button SFX is not assigned.");
        }

        protected override void OnButtonClick()
        {
            PlayButtonSound(_buttonSFX);
        }
    }
}
