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
        protected SnekSFXManager _sfxManager;

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
        }

        protected void PlayButtonSound(AudioClip sound)
        {
            if (_useUnmutableAudioSource)
                _sfxManager.PlayUnmutableSound(sound);
            else
                _sfxManager.PlaySound(sound);
        }
    }
}
