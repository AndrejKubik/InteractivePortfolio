using System;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

namespace Snek.AudioManager
{
    public class SnekAudioManager : SnekMonoSingleton
    {
        protected const float DefaultVolume = 0.5f;

        protected AudioSource _audioSource;

        public bool DefaultMuteState = false;

        protected override void OnInitialize()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void Validate()
        {
            if (!_audioSource)
                FailValidation("Cannot find AudioSource component.");
        }

        public virtual void SetVolume(float newVolume)
        {
            newVolume = Mathf.Clamp01(newVolume);

            _audioSource.volume = newVolume;
        }

        public float GetVolume()
        {
            return _audioSource.volume;
        }

        public void SetMute(bool newState)
        {
            _audioSource.mute = newState;
        }

        protected int GetDefaultMuteStateAsInt()
        {
            return Convert.ToInt32(DefaultMuteState);
        }

        public bool IsMuted()
        {
            return _audioSource.mute;
        }
    }
}
