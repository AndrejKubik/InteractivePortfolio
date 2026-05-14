using UnityEngine;
using Snek.SingletonManager;
using Snek.GameUI;
using Snek.AudioManager;

namespace Snek.GameUIPlus
{
    public abstract class SnekUISwitchWithSFX : SnekUISwitch
    {
        private SnekSFXManager _sfxManager;
        
        [Space(10f)]
        [SerializeField] private bool _useUnmutableAudioSource;

        [Space(10f)]
        [SerializeField] private AudioClip _switchEnableSound;
        [SerializeField] private AudioClip _switchDisableSound;

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

            if (!_switchEnableSound)
                FailValidation("Switch enable sound is not assigned.");

            if (!_switchDisableSound)
                FailValidation("Switch disable sound is not assigned.");
        }

        protected override void OnSwitchEnable()
        {
            if (_useUnmutableAudioSource)
                _sfxManager.PlayUnmutableSound(_switchEnableSound);
            else 
                _sfxManager.PlaySound(_switchEnableSound);
        }

        protected override void OnSwitchDisable()
        {
            if (_useUnmutableAudioSource)
                _sfxManager.PlayUnmutableSound(_switchDisableSound);
            else
                _sfxManager.PlaySound(_switchDisableSound);
        }
    }
}
