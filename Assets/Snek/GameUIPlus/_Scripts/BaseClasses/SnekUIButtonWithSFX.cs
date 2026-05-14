using Snek.Utilities;
using UnityEngine;
using Snek.SingletonManager;
using Snek.GameUI;
using Snek.AudioManager;

namespace Snek.GameUIPlus
{
    [UseSnekInspector]
    public class SnekUIButtonWithSFX : SnekUIButton
    {
        private SnekSFXManager _sfxManager;

        [SerializeField] private AudioClip _buttonSFX;
        [SerializeField] private bool _useUnmutableAudioSource;

        protected override void Initialize()
        {
            base.Initialize();

            _sfxManager = SnekSingletonManager.GetSingleton<SnekSFXManager>();
        }

        protected override void Validate()
        {
            base.Validate();

            if (!_sfxManager)
                FailValidation("Cannot find SnekSFXManager singleton.");

            if (!_buttonSFX)
                FailValidation("Button SFX is not assigned.");
        }

        protected override void OnButtonClick()
        {
            if (_useUnmutableAudioSource)
                _sfxManager.PlayUnmutableSound(_buttonSFX);
            else
                _sfxManager.PlaySound(_buttonSFX);
        }
    }
}
