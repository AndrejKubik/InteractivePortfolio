using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Snek.EndlessCarousel
{
    [UseSnekInspector]
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter))]
    public class SnekEndlessCarouselElementContainer : SnekMonoBehaviour, ISnekInitializableExternal<SnekEndlessCarouselElementContainer.Data>
    {
        public readonly struct Data
        {
            public readonly bool IsScrollable;

            public Data(bool isScrollable)
            {
                IsScrollable = isScrollable;
            }
        }

        [Tooltip("Where should elements be anchored to if they can all be visible at the same time?")]
        [SerializeField] private TextAnchor _noScrollElementAlignment = TextAnchor.MiddleCenter;

        private RectTransform _rectTransform;
        private GridLayoutGroup _gridLayoutGroup;
        private ContentSizeFitter _contentSizeFitter;

        private bool _isScrollable = true;

        public void OnBeforeInitialize(Data data)
        {
            _isScrollable = data.IsScrollable;
        }

        protected override void Initialize()
        {
            GetEssentialComponent(out _rectTransform);
            GetEssentialComponent(out _gridLayoutGroup);
            GetEssentialComponent(out _contentSizeFitter);
        }

        protected override void Validate()
        {
            if (_gridLayoutGroup.enabled == false)
                FailValidation("Grid Layout Group is disabled, it must be enabled by default for correct item distribution.");

            if (_contentSizeFitter.enabled == false)
                FailValidation("Content size fitter is disabled, it must be enabled by default for correct item distribution.");
        }

        protected override void OnInitializationSuccess()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform); //enforces element position distribution before disabling the layout group

            _contentSizeFitter.enabled = false;

            if (_isScrollable)
                _gridLayoutGroup.enabled = false;
            else
            {
                _rectTransform.ResetAnchorOffset();
                _gridLayoutGroup.childAlignment = _noScrollElementAlignment;
            }
        }

        public float GetTotalWidth()
        {
            return _rectTransform.rect.width;
        }
    }
}