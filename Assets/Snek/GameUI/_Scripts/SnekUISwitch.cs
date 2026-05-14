using Snek.Utilities;

namespace Snek.GameUI
{
    public abstract class SnekUISwitch : SnekMonoBehaviour
    {
        private SnekUIButton _disabledStateButton;
        private SnekUIButton _enabledStateButton;

        public bool IsOn { get; private set; }

        protected override void Initialize()
        {
            SnekUISwitchButton[] buttons = GetComponentsInChildren<SnekUISwitchButton>(true);

            foreach (SnekUISwitchButton button in buttons)
                if (button.EnabledState == false)
                    _disabledStateButton = button;
                else if (button.EnabledState == true)
                    _enabledStateButton = button;
        }

        protected override void Validate()
        {
            if (!_disabledStateButton)
                FailValidation("OFF button not assigned.");

            if (!_enabledStateButton)
                FailValidation("ON button not assigned.");
        }

        protected override void OnInitializationSuccess()
        {
            _disabledStateButton.SetExternalCallback(OnDisabledStateButtonClick);
            _enabledStateButton.SetExternalCallback(OnEnabledStateButtonClick);
        }

        protected abstract void OnSwitchDisable();

        protected abstract void OnSwitchEnable();

        private void OnDisabledStateButtonClick()
        {
            SetEnabledState(true);
        }

        private void OnEnabledStateButtonClick()
        {
            SetEnabledState(false);
        }

        protected void SetEnabledState(bool newState, bool ignoreCallback = false)
        {
            if (newState == true)
            {
                _enabledStateButton.gameObject.SetActive(true);
                _disabledStateButton.gameObject.SetActive(false);

                IsOn = true;

                if (!ignoreCallback)
                    OnSwitchEnable();
            }
            else if (newState == false)
            {
                _enabledStateButton.gameObject.SetActive(false);
                _disabledStateButton.gameObject.SetActive(true);

                IsOn = false;

                if (!ignoreCallback)
                    OnSwitchDisable();
            }
        }
    }
}
