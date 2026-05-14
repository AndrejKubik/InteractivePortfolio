using Snek.Utilities;
using UnityEngine;
using Snek.SingletonManager;
using Snek.GameUIPlus;
using Snek.AudioManager;

namespace Snek.SettingsMenu
{
    [UseSnekInspector]
    public class SnekAudioVolumeSlider : SnekUISliderWithSFX
    {
        private SnekAudioManager _audioSourceManager;

        [SerializeField] private SnekAudioType _audioType = SnekAudioType.SFX;

        protected override void Initialize()
        {
            base.Initialize();

            if (_audioType == SnekAudioType.SFX)
                _audioSourceManager = SnekSingletonManager.GetSingleton<SnekSFXManager>();
            else if (_audioType == SnekAudioType.Music)
                _audioSourceManager = SnekSingletonManager.GetSingleton<SnekMusicManager>();
        }

        protected override void Validate()
        {
            base.Validate();

            if (!_audioSourceManager)
                FailValidation("Cannot find target SnekAudioSourceManager component.");
        }

        private void OnEnable()
        {
            Slider.SetValueWithoutNotify(_audioSourceManager.GetVolume());
        }

        protected override void OnSliderMove(float newValue)
        {
            _audioSourceManager.SetVolume(newValue);

            SaveVolumeToPlayerPrefs(newValue);
        }

        private void SaveVolumeToPlayerPrefs(float newValue)
        {
            string playerPrefsId = string.Empty;

            if (_audioType == SnekAudioType.SFX)
                playerPrefsId = SnekSFXManager.PlayerPrefsVolumeID;
            else if (_audioType == SnekAudioType.Music)
                playerPrefsId = SnekMusicManager.PlayerPrefsVolumeID;

            if (string.IsNullOrEmpty(playerPrefsId))
                Debug.LogError("Something went wrong, cannot save volume to player prefs");

            PlayerPrefs.SetFloat(playerPrefsId, newValue);
        }
    }
}
