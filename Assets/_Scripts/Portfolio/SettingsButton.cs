using Snek.GameUI;
using Snek.SingletonManager;
using Snek.Utilities;
using SnekEditor.SettingsMenu;

[UseSnekInspector]
public class SettingsButton : SnekUIButton
{
    private SnekSettingsMenu _settingsMenu;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        SnekSingletonManager.GetSingleton(out _settingsMenu);
    }

    protected override void OnButtonClick()
    {
        _settingsMenu.ShowMenu(true);
    }
}
