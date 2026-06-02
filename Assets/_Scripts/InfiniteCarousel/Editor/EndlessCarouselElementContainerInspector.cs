using SnekEditor.GUIUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(EndlessCarouselElementContainer))]
public class EndlessCarouselElementContainerInspector : SnekMonoBehaviourInspectorCustom<EndlessCarouselElementContainer>
{
    private EndlessCarousel _endlessCarousel;
    private EndlessCarouselElementContainer _elementContainer;

    private GridLayoutGroup _gridLayoutGroup;
    private ContentSizeFitter _contentSizeFitter;

    protected override void OnCreateInspectorInstance()
    {
        _elementContainer = GetSelectedComponent();
        _endlessCarousel = _elementContainer.GetComponentInParent<EndlessCarousel>();

        if (IsUsedByEndlessCarousel())
            InitializeDependencies();
    }

    protected override void DrawContent()
    {
        if (IsUsedByEndlessCarousel())
            SnekGUILayout.DrawAlertMessage(
                $"Following components are controlled by {nameof(EndlessCarousel)}:\n\n" +
                $"- {nameof(GridLayoutGroup)}\n" +
                $"- {nameof(ContentSizeFitter)}");
        else
            SnekGUILayout.DrawAlertMessage($"Unused by an {nameof(EndlessCarousel)} component.");
    }

    private void OnDisable()
    {
        if (!_endlessCarousel)
            return;

        if (_gridLayoutGroup)
            _gridLayoutGroup.hideFlags = HideFlags.None;

        if (_contentSizeFitter)
            _contentSizeFitter.hideFlags = HideFlags.None;
    }

    private void InitializeDependencies()
    {
        if (_elementContainer.TryGetComponent(out _gridLayoutGroup))
            _gridLayoutGroup.hideFlags = HideFlags.NotEditable;

        if (_elementContainer.TryGetComponent(out _contentSizeFitter))
            _contentSizeFitter.hideFlags = HideFlags.NotEditable;
    }

    private bool IsUsedByEndlessCarousel()
    {
        return _endlessCarousel && _endlessCarousel.ElementContainer == _elementContainer;
    }
}
