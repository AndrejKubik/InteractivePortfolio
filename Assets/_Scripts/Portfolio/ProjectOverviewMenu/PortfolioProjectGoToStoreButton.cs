using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectGoToStoreButton : SnekUIButton, ISnekInitializableWithData<PortfolioProjectGoToStoreButton.Data>
{
    public readonly struct Data
    {
        public readonly string StorePageURL;

        public Data(string storePageURL)
        {
            StorePageURL = storePageURL;
        }
    }

    private string _storePageURL = string.Empty;

    public void PrepareInitializationData(Data data)
    {
        _storePageURL = data.StorePageURL;
    }

    protected override void Validate()
    {
        base.Validate();

        if (string.IsNullOrWhiteSpace(_storePageURL))
            FailValidation("No store page URL provided.");
    }

    protected override void OnButtonClick()
    {
        Application.OpenURL(_storePageURL);
    }
}
