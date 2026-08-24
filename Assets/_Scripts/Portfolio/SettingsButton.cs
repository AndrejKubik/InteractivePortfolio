using Snek.GameUI;
using Snek.SingletonManager;
using Snek.Utilities;
using SnekEditor.SettingsMenu;

[UseSnekInspector]
public class SettingsButton : SnekUIButton
{
    private SnekSettingsMenu _settingsMenu;

    protected override void Initialize()
    {
        base.Initialize();

        SnekSingletonManager.GetSingleton(out _settingsMenu);
    }

    protected override void OnButtonClick()
    {
        _settingsMenu.ShowMenu(true);
    }
}
