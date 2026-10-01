using System;
using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectVideoHoverOverlayController : SnekMonoBehaviour, ISnekInitializableWithData<PortfolioProjectVideoHoverOverlayController.Data>
{
    public readonly struct Data
    {
        public readonly Action OnOverlayButtonClick;
        public readonly Action OnShowOverlay;

        public Data(Action onOverlayButtonClick, Action onShowOverlay)
        {
            OnOverlayButtonClick = onOverlayButtonClick;
            OnShowOverlay = onShowOverlay;
        }
    }

    [SerializeField] private HoverOverlay _hoverOverlay;
    [SerializeField] private PortfolioProjectVideoDemoOverlayButton _overlayButton;

    [Min(0f)]
    [SerializeField] private float _overlayButtonSymbolFadeTime = 0.5f;

    private Action _onOverlayButtonClick;
    private Action _onShowOverlay;

    private float _symbolCurrentAlpha = 0f;

    public void PrepareInitializationData(Data data)
    {
        _onOverlayButtonClick = data.OnOverlayButtonClick;
        _onShowOverlay = data.OnShowOverlay;
    }

    protected override void Validate()
    {
        ValidateEssentialComponent(_hoverOverlay, nameof(_hoverOverlay));
        ValidateEssentialComponent(_overlayButton, nameof(_overlayButton));
    }

    protected override void OnInitializationSuccess()
    {
        _hoverOverlay.Initialize(new HoverOverlay.Data(_onShowOverlay));

        _overlayButton.SetExternalCallback(_onOverlayButtonClick);
    }

    private void Update()
    {
        if (_symbolCurrentAlpha > 0f)
            FadeOverlaySymbol();
    }

    public void ControlHoverOverlayUserActivityBased(bool isFadeAllowed)
    {
        _hoverOverlay.HandleMouseHoverUserActivityBased(isFadeAllowed);
    }

    public void ControlHoverOverlayConstant()
    {
        _hoverOverlay.HandleMouseHoverConstant();
    }

    private void FadeOverlaySymbol()
    {
        _symbolCurrentAlpha = Mathf.MoveTowards(
            _symbolCurrentAlpha,
            0f,
            Time.deltaTime / _overlayButtonSymbolFadeTime);

        _overlayButton.SetSymbolAlpha(_symbolCurrentAlpha);
    }

    public void ShowFadingOverlay(Sprite sprite)
    {
        _symbolCurrentAlpha = 1f;

        _overlayButton.SetSymbol(sprite);
        _overlayButton.SetSymbolAlpha(_symbolCurrentAlpha);

        _hoverOverlay.Show();
    }
}
