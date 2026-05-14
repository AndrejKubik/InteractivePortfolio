using System;
using Snek.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Snek.AudioManager
{
    [UseSnekInspector]
    public class SnekSFXManager : SnekAudioManager
    {
        public const string PlayerPrefsVolumeID = "SnekAudioSFXVolume";
        public const string PlayerPrefsMuteID = "SnekAudioSFXMute";

        public AudioSource UnmutableAudioSource;
        
        [Space(10f)]
        [Min(0f)]
        [SerializeField] private float _pitchRandomizeRangeSize = 0.05f;

        private float _defaultPitch;

        protected override void Validate()
        {
            base.Validate();

            if (!UnmutableAudioSource)
                FailValidation("Unmutable audio source not assigned.");
        }

        protected override void OnInitializationSuccess()
        {
            SetVolume(PlayerPrefs.GetFloat(PlayerPrefsVolumeID, 0.5f));
            SetMute(Convert.ToBoolean(PlayerPrefs.GetInt(PlayerPrefsMuteID, 0)));

            _defaultPitch = _audioSource.pitch;
        }

        public void PlayRandomPitchSound(AudioClip audioClip, float volumeScale = 1f)
        {
            float minPitch = _defaultPitch - _pitchRandomizeRangeSize;
            float maxPitch = _defaultPitch + _pitchRandomizeRangeSize;

            _audioSource.pitch = Random.Range(minPitch, maxPitch);

            _audioSource.PlayOneShot(audioClip, volumeScale);
        }

        public void PlaySound(AudioClip audioClip, float volumeScale = 1f)
        {
            _audioSource.pitch = _defaultPitch;

            _audioSource.PlayOneShot(audioClip, volumeScale);
        }

        /// <summary>
        /// Plays the audio clip regardless of the SFX being muted or not
        /// </summary>
        public void PlayUnmutableSound(AudioClip audioClip, float volumeScale = 1f)
        {
            UnmutableAudioSource.PlayOneShot(audioClip, volumeScale);
        }

        public override void SetVolume(float newVolume)
        {
            base.SetVolume(newVolume);

            UnmutableAudioSource.volume = _audioSource.volume;
        }
    }
}
