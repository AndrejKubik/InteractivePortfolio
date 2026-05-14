using Snek.GameUI;
using UnityEngine;
using Snek.Utilities;
using Snek.SingletonManager;
using Snek.AudioManager;

namespace Snek.GameUIPlus
{
    [UseSnekInspector]
    public abstract class SnekUISliderWithSFX : SnekUISlider
    {
        private SnekSFXManager _sfxManager;

        public AudioClip GrabSound;
        public AudioClip DragSound;
        public AudioClip ReleaseSound;

        public float SFXCooldown = 0.1f;

        private float _sfxRemainingCooldown = 0f;

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

            if (!GrabSound)
                FailValidation("Slider grab sound is not assigned.");

            if (!DragSound)
                FailValidation("Slider drag sound is not assigned.");

            if (!ReleaseSound)
                FailValidation("Slider release sound is not assigned.");
        }

        protected virtual void Update()
        {
            if (IsHandleHeld)
                UpdateCooldownTimer();
        }

        protected override void OnHandleGrab()
        {
            RequestSFX(GrabSound);
        }

        protected override void OnDragThresholdReach()
        {
            RequestSFX(DragSound);
        }

        protected override void OnDragAreaChange()
        {
            RequestSFX(DragSound);
        }

        protected override void OnHandleRelease()
        {
            RequestSFX(ReleaseSound);
        }

        private void RequestSFX(AudioClip sound)
        {
            if (IsSFXOnCooldown())
                return;

            _sfxManager.PlaySound(sound);

            StartCooldown();
        }

        private void UpdateCooldownTimer()
        {
            _sfxRemainingCooldown -= Time.unscaledDeltaTime;
            _sfxRemainingCooldown = Mathf.Max(0f, _sfxRemainingCooldown);
        }

        private bool IsSFXOnCooldown()
        {
            return _sfxRemainingCooldown > 0f;
        }

        private void StartCooldown()
        {
            _sfxRemainingCooldown = SFXCooldown;
        }
    }
}
