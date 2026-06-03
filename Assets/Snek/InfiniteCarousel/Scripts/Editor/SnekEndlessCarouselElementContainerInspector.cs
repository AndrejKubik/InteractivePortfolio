using Snek.EndlessCarousel;
using SnekEditor.GUIUtilities;
using Snek.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SnekEditor.EndlessCarousel
{
    [CustomEditor(typeof(SnekEndlessCarouselElementContainer))]
    public class SnekEndlessCarouselElementContainerInspector : SnekMonoBehaviourInspectorCustom<SnekEndlessCarouselElementContainer>
    {
        private SnekEndlessCarousel _endlessCarousel;
        private SnekEndlessCarouselElementContainer _elementContainer;

        private GridLayoutGroup _gridLayoutGroup;
        private ContentSizeFitter _contentSizeFitter;

        protected override void OnCreateInspectorInstance()
        {
            _elementContainer = GetSelectedComponent();
            _endlessCarousel = _elementContainer.GetComponentInParent<SnekEndlessCarousel>();

            if (IsUsedByEndlessCarousel())
                InitializeDependencies();
        }

        protected override void DrawContent()
        {
            if (IsUsedByEndlessCarousel())
                SnekGUILayout.DrawAlertMessage(
                    $"Following components are controlled by {nameof(SnekEndlessCarousel).Nicify()}:\n\n" +
                    $"- {nameof(GridLayoutGroup).Nicify()}\n" +
                    $"- {nameof(ContentSizeFitter).Nicify()}");
            else
                SnekGUILayout.DrawAlertMessage($"Unused by an {nameof(SnekEndlessCarousel)} component.");
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
}