using System;
using System.Collections;
using System.Collections.Generic;
using Snek.Utilities;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Snek.AudioManager
{
    [UseSnekInspector]
    public class SnekMusicManager : SnekAudioManager
    {
        public const string PlayerPrefsVolumeID = "SnekAudioMusicVolume";
        public const string PlayerPrefsMuteID = "SnekAudioMusicMute";

        public List<AudioClip> MusicTracks = new List<AudioClip>();
        public bool ShuffleTracks = false;

        public bool RandomDefaultTrack = false;
        public int DefaultTrack = 0;

        public bool FadeBetweenTracks = true;
        public float FadeTransitionDuration = 0.35f;

        private Coroutine _activeTransition;
        private float _baseVolume = 1f;

        private List<int> _playOrder = new List<int>();
        private int _playOrderCurrentIndex = 0;

        private struct MusicTrackData
        {
            public string Name;
            public float Duration;
        }

        private bool _isMusicPlaying = false;

        private MusicTrackData _currentTrack;

        public Action<string> OnTrackChanged;

        protected override void Validate()
        {
            base.Validate();

            if (!MusicTracks.HasIndex(DefaultTrack) || MusicTracks[DefaultTrack] == null)
                FailValidation("Assigned default music track is invalid.");
        }

        protected override void OnInitializationSuccess()
        {
            float savedVolume = PlayerPrefs.GetFloat(PlayerPrefsVolumeID, DefaultVolume);
            bool savedMuteState = Convert.ToBoolean(PlayerPrefs.GetInt(PlayerPrefsMuteID, 0));

            SetVolume(savedVolume);
            SetMute(savedMuteState);

            _audioSource.volume = _baseVolume;

            ClearNullPlaylistTracks();
        }

        private void Update()
        {
            if (!_isMusicPlaying)
                return;

            if (GetCurrentTrackProgress() >= 1f)
                PlayNextTrack();
        }

        public override void SetVolume(float newVolume)
        {
            _baseVolume = newVolume;

            if (!IsChangingTrack())
                base.SetVolume(newVolume);
        }

        private void ClearNullPlaylistTracks()
        {
            bool nullDetected = false;

            for (int i = MusicTracks.Count - 1; i >= 0; i--)
            {
                if (MusicTracks[i] != null)
                    continue;

                nullDetected = true;

                if (i < DefaultTrack)
                    DefaultTrack--;

                MusicTracks.RemoveAt(i);
            }

            if (nullDetected)
                Debug.Log("Null music tracks detected in the playlist, please clear them in the inspector to avoid potential issues. ");
        }

        public void StartPlaylist()
        {
            if (MusicTracks.Count < 1)
            {
                Debug.Log("No music tracks assigned, nothing to play...");

                return;
            }

            GenerateTrackPlayOrder();

            if (ShuffleTracks)
            {
                if (!RandomDefaultTrack)
                    SetDefaultTrackAsFirstInPlayOrder();

                PlayTrack(_playOrder[0]);
            }
            else
            {
                int startingTrackIndex = RandomDefaultTrack ? MusicTracks.GetRandomIndex() : DefaultTrack;

                PlayTrack(startingTrackIndex);
            }
        }

        public void StopPlaylist()
        {
            if (IsChangingTrack())
                StopActiveTransition();

            _isMusicPlaying = false;
            _audioSource.Stop();
        }

        public void PlayNextTrack()
        {
            if (!_isMusicPlaying)
            {
                Debug.Log("Music playlist is not currently playing, cannot switch track.");

                return;
            }

            int nextOrderIndex = _playOrderCurrentIndex + 1;

            if (nextOrderIndex >= _playOrder.Count)
            {
                if (ShuffleTracks)
                    GenerateTrackPlayOrder(true);

                nextOrderIndex = 0;
            }

            PlayTrack(_playOrder[nextOrderIndex]);
        }

        public void PlayPreviousTrack()
        {
            if (!_isMusicPlaying)
            {
                Debug.Log("Music playlist is not currently playing, cannot switch track.");

                return;
            }

            int previousOrderIndex = _playOrderCurrentIndex - 1;

            if (previousOrderIndex < 0)
                previousOrderIndex = _playOrder.Count - 1;

            PlayTrack(_playOrder[previousOrderIndex]);
        }

        private void GenerateTrackPlayOrder(bool avoidImmediateRepeat = false)
        {
            _playOrder = new List<int>(MusicTracks.Count);

            for (int i = 0; i < MusicTracks.Count; i++)
                _playOrder.Add(i);

            if (ShuffleTracks)
                _playOrder.Shuffle();

            if (avoidImmediateRepeat)
                PreventShuffleImmediateRepeat();
        }

        private void SetDefaultTrackAsFirstInPlayOrder()
        {
            _playOrder.Remove(DefaultTrack);
            _playOrder.Insert(0, DefaultTrack);
        }

        private float GetCurrentTrackProgress()
        {
            return Mathf.InverseLerp(0f, _currentTrack.Duration, _audioSource.time);
        }

        private int GetTrackPlayOrderIndex(int unshuffledPlaylistIndex)
        {
            int playOrderIndex = _playOrder.IndexOf(unshuffledPlaylistIndex);

            if (playOrderIndex < 0)
            {
                Debug.Log(
                    "Track not found in current play order.\n" +
                    "Generated new track play order.");

                GenerateTrackPlayOrder();

                playOrderIndex = _playOrder.IndexOf(unshuffledPlaylistIndex);
            }

            return playOrderIndex;
        }

        public string GetCurrentTrackName()
        {
            return _currentTrack.Name;
        }

        private bool IsTrackPlayOrderValid()
        {
            return _playOrder != null && _playOrder.Count == MusicTracks.Count;
        }

        private void PreventShuffleImmediateRepeat()
        {
            if (_playOrder.Count < 2)
                return;

            int nextTrack = _playOrder[0];
            int lastPlayedTrack = _playOrder[_playOrderCurrentIndex];

            if (nextTrack == lastPlayedTrack)
            {
                int swapIndex = Random.Range(1, _playOrder.Count);

                _playOrder.SwapListElements(0, swapIndex);
            }
        }

        /// <summary>
        /// Plays a track by given index from the <c>un-shuffled</c> playlist
        /// </summary>
        public void PlayTrack(int trackIndex)
        {
            if (!MusicTracks.HasIndex(trackIndex))
            {
                Debug.LogError("Cannot play music track, playlist does not contain requested index.");

                return;
            }

            AudioClip audioClip = MusicTracks[trackIndex];

            if (!audioClip)
            {
                Debug.LogError("Trying to play a null audio track, aborting...");

                return;
            }

            if (!IsTrackPlayOrderValid())
                GenerateTrackPlayOrder();

            _playOrderCurrentIndex = GetTrackPlayOrderIndex(trackIndex);

            if (!FadeBetweenTracks || FadeTransitionDuration <= 0f)
            {
                PlayAudioClip(audioClip);

                return;
            }

            if (IsChangingTrack())
                StopActiveTransition();

            _activeTransition = StartCoroutine(TransitionToAudioClip(audioClip));
        }

        private IEnumerator TransitionToAudioClip(AudioClip audioClip)
        {
            yield return StartCoroutine(FadeVolume(_audioSource.volume, 0f, FadeTransitionDuration * 0.5f));

            PlayAudioClip(audioClip);

            yield return StartCoroutine(FadeVolume(0f, _baseVolume, FadeTransitionDuration * 0.5f));

            _activeTransition = null;
        }

        private void PlayAudioClip(AudioClip audioClip)
        {
            _currentTrack = new MusicTrackData()
            {
                Name = audioClip.name,
                Duration = audioClip.length
            };

            _audioSource.clip = audioClip;
            _audioSource.Play();

            _isMusicPlaying = true;

            OnTrackChanged?.Invoke(GetCurrentTrackName());
        }

        private IEnumerator FadeVolume(float startValue, float targetValue, float duration)
        {
            if (duration <= 0f)
            {
                _audioSource.volume = targetValue;

                yield break;
            }

            float elapsedTime = 0f;
            float t = 0f;

            while (t < 1)
            {
                t = Mathf.Clamp01(elapsedTime / duration);
                _audioSource.volume = Mathf.Lerp(startValue, targetValue, t);

                elapsedTime += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        private bool IsChangingTrack()
        {
            return _activeTransition != null;
        }

        private void StopActiveTransition()
        {
            StopCoroutine(_activeTransition);
            _activeTransition = null;

            _audioSource.volume = _baseVolume;
        }
    }
}
