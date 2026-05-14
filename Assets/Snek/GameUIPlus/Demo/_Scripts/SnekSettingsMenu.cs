using Snek.GameUIPlus;
using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.SettingsMenu
{
    [UseSnekInspector]
    public class SnekSettingsMenu : SnekMonoSingleton
    {
        [SerializeField] private GameObject _background;
        [SerializeField] private GameObject _mainPanel;

        [SerializeField] private SnekUIButtonWithSFX _closeMenuButton;

        public bool IsVisible { get; private set; }

        protected override void Validate()
        {
            if (!_background)
                FailValidation("Background object is not assigned.");

            if (!_mainPanel)
                FailValidation("Main Panel object is not assigned.");

            if (_closeMenuButton)
                _closeMenuButton.SetExternalCallback(OnCloseMenuButtonClick);
            else
                FailValidation("Close Menu Button is not assigned.");
        }

        private void OnCloseMenuButtonClick()
        {
            ShowMenu(false);
        }

        public void ShowMenu(bool newState)
        {
            _background.SetActive(newState);
            _mainPanel.SetActive(newState);

            IsVisible = newState;
        }
    }
}
