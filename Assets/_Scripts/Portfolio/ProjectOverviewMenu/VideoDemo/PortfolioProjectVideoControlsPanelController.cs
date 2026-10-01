using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
public class PortfolioProjectVideoControlsPanelController : SnekMonoBehaviour
{
    [SerializeField] private RectTransform _controlsPanel;

    [Min(0f)]
    [SerializeField] private float _fullscreenHoverControlsPanelShowTimeMax = 3f;

    [SerializeField] private AnimationCurve _fullscreenControlsPanelHideCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private float _controlsPanelHeight = 0f;
    private float _fullscreenHoverControlsPanelShowTime = 0f;

    protected override void Validate()
    {
        ValidateEssentialComponent(_controlsPanel, nameof(_controlsPanel));
    }

    protected override void OnInitializationSuccess()
    {
        _controlsPanelHeight = _controlsPanel.rect.height;
    }

    public void ControllFullscreenControlsPanelAnimation()
    {
        float showProgress = _fullscreenHoverControlsPanelShowTime / _fullscreenHoverControlsPanelShowTimeMax;
        float showProgressCurved = _fullscreenControlsPanelHideCurve.Evaluate(showProgress);

        float hidePositionY = 0f;
        float showPositionY = _controlsPanelHeight;

        float targetPositionY = Mathf.Lerp(hidePositionY, showPositionY, showProgressCurved);

        _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, targetPositionY);

        _fullscreenHoverControlsPanelShowTime += Time.deltaTime;

        _fullscreenHoverControlsPanelShowTime = Mathf.Min(
            _fullscreenHoverControlsPanelShowTime,
            _fullscreenHoverControlsPanelShowTimeMax);
    }

    public void ShowFullscreenControlPanel()
    {
        MoveControlsPanelToFullscreenPlayerDefaultPosition();

        _fullscreenHoverControlsPanelShowTime = 0f;
    }

    public void MoveControlsPanelToMiniPlayerDefaultPosition()
    {
        _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, 0f);
    }

    private void MoveControlsPanelToFullscreenPlayerDefaultPosition()
    {
        _controlsPanel.anchoredPosition = new Vector2(_controlsPanel.anchoredPosition.x, _controlsPanelHeight);
    }

    public bool IsControlsPanelVisible()
    {
        return _fullscreenHoverControlsPanelShowTime < _fullscreenHoverControlsPanelShowTimeMax;
    }
}
