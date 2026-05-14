using UnityEngine;
using Snek.Utilities;
using Snek.SingletonManager;
using Snek.AudioManager;

namespace SnekEditor.AudioManager
{
    [UseSnekInspector]
    public class SnekAudioManagerTester : SnekMonoBehaviour
    {
        private SnekSFXManager _sfxManager;
        private SnekMusicManager _musicManager;

        [SerializeField] private AudioClip _testClip;

        protected override void Initialize()
        {
            _sfxManager = SnekSingletonManager.GetSingleton<SnekSFXManager>();
            _musicManager = SnekSingletonManager.GetSingleton<SnekMusicManager>();
        }

        protected override void Validate()
        {
            if (!_sfxManager)
                FailValidation("Cannot find SnekSFXManager Singleton.");

            if (!_musicManager)
                FailValidation("Cannot find SnekMusicManager Singleton.");
        }

        protected override void OnInitializationSuccess()
        {
            _musicManager.StartPlaylist();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _sfxManager.PlaySound(_testClip);
            else if (Input.GetKeyDown(KeyCode.N))
                _musicManager.PlayNextTrack();
            else if (Input.GetKeyDown(KeyCode.B))
                _musicManager.PlayPreviousTrack();
        }
    }
}
