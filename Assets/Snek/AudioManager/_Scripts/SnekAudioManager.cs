using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

namespace Snek.AudioManager
{
    public class SnekAudioManager : SnekMonoSingleton
    {
        protected const float DefaultVolume = 0.5f;

        protected AudioSource _audioSource;

        protected override void Initialize()
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

        public bool IsMuted()
        {
            return _audioSource.mute;
        }
    }
}
