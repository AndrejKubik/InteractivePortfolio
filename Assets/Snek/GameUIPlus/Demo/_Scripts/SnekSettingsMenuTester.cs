using Snek.SingletonManager;
using Snek.Utilities;
using UnityEngine;

namespace SnekEditor.SettingsMenu
{
    [UseSnekInspector]
    public class SnekSettingsMenuTester : SnekMonoBehaviour
    {
        private SnekSettingsMenu _settingsMenu;

        [SerializeField] private KeyCode _menuToggleHotkey = KeyCode.Escape;

        protected override void Initialize()
        {
            _settingsMenu = SnekSingletonManager.GetSingleton<SnekSettingsMenu>();
        }

        protected override void Validate()
        {
            if (!_settingsMenu)
                FailValidation("Cannot find SnekSettingsMenu singleton.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(_menuToggleHotkey))
                _settingsMenu.ShowMenu(!_settingsMenu.IsVisible);
        }
    }
}
