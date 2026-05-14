using System;
using Snek.Utilities;
using UnityEngine;
using Snek.SingletonManager;
using Snek.GameUIPlus;
using Snek.AudioManager;

namespace Snek.SettingsMenu
{
    [UseSnekInspector]
    public class SnekAudioSwitch : SnekUISwitchWithSFX
    {
        private SnekAudioManager _targetAudioManager;

        [SerializeField] private SnekAudioType _audioType = SnekAudioType.SFX;

        protected override void Initialize()
        {
            base.Initialize();

            if (_audioType == SnekAudioType.SFX)
                _targetAudioManager = SnekSingletonManager.GetSingleton<SnekSFXManager>();
            else if (_audioType == SnekAudioType.Music)
                _targetAudioManager = SnekSingletonManager.GetSingleton<SnekMusicManager>();
        }

        protected override void Validate()
        {
            base.Validate();

            if (!_targetAudioManager)
                FailValidation("Cannot find target SnekAudioSourceManager component.");
        }

        private void OnEnable()
        {
            SetEnabledState(!_targetAudioManager.IsMuted(), true);
        }

        protected override void OnSwitchEnable()
        {
            base.OnSwitchEnable();

            _targetAudioManager.SetMute(false);

            SaveStateToPlayerPrefs();
        }

        protected override void OnSwitchDisable()
        {
            base.OnSwitchDisable();

            _targetAudioManager.SetMute(true);

            SaveStateToPlayerPrefs();
        }

        private void SaveStateToPlayerPrefs()
        {
            string playerPrefsId = string.Empty;

            if (_audioType == SnekAudioType.SFX)
                playerPrefsId = SnekSFXManager.PlayerPrefsMuteID;
            else if (_audioType == SnekAudioType.Music)
                playerPrefsId = SnekMusicManager.PlayerPrefsMuteID;

            if (string.IsNullOrEmpty(playerPrefsId))
                Debug.LogError("Something went wrong, cannot save volume to player prefs");

            PlayerPrefs.SetInt(playerPrefsId, Convert.ToInt32(_targetAudioManager.IsMuted()));
        }
    }
}
