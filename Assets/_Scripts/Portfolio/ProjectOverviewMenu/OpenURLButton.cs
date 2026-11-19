using Snek.GameUI;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class OpenURLButton : SnekUIButton, ISnekInitializableWithData<OpenURLButton.Data>
{
    public readonly struct Data
    {
        public readonly string URL;

        public Data(string url)
        {
            URL = url;
        }
    }

    private string _url = string.Empty;

    public void PrepareInitializationData(Data data)
    {
        _url = data.URL;
    }

    protected override void Validate()
    {
        base.Validate();

        if (string.IsNullOrWhiteSpace(_url))
            FailValidation("No store page URL provided.");
    }

    protected override void OnButtonClick()
    {
        Application.OpenURL(_url);
    }
}
