using Snek.AudioManager;
using Snek.GameUIPlus;
using Snek.SingletonManager;
using Snek.Utilities;
using TMPro;
using UnityEngine;

namespace SnekEditor.SettingsMenu
{
    [UseSnekInspector]
    public class SnekMusicTrackManager : SnekMonoBehaviour
    {
        private SnekMusicManager _musicManager;

        [SerializeField] private TextMeshProUGUI _trackName;

        [Space(10f)]
        [SerializeField] private SnekUIButtonWithSFX _nextTrackButton;
        [SerializeField] private SnekUIButtonWithSFX _previousTrackButton;

        protected override void Initialize()
        {
            _musicManager = SnekSingletonManager.GetSingleton<SnekMusicManager>();
        }

        protected override void Validate()
        {
            if (!_trackName)
                FailValidation("Music track name is not assigned.");

            if (_nextTrackButton)
                _nextTrackButton.SetExternalCallback(OnNextTrackClick);
            else
                FailValidation("Next music track button is not assigned.");

            if (_previousTrackButton)
                _previousTrackButton.SetExternalCallback(OnPreviousTrackClick);
            else
                FailValidation("Previous music track button is not assigned.");
        }

        protected override void OnInitializationSuccess()
        {
            _musicManager.OnTrackChanged += OnMusicTrackChange;
        }

        private void OnDestroy()
        {
            _musicManager.OnTrackChanged -= OnMusicTrackChange;
        }

        private void OnEnable()
        {
            _trackName.SetText(_musicManager.GetCurrentTrackName());
        }

        private void OnNextTrackClick()
        {
            _musicManager.PlayNextTrack();
        }

        private void OnPreviousTrackClick()
        {
            _musicManager.PlayPreviousTrack();
        }

        private void OnMusicTrackChange(string trackName)
        {
            _trackName.SetText(trackName);
        }
    }
}
