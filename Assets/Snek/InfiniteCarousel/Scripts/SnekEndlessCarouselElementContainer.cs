using Snek.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Snek.EndlessCarousel
{
    [UseSnekInspector]
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter))]
    public class SnekEndlessCarouselElementContainer : SnekMonoBehaviour, ISnekInitializableWithData<SnekEndlessCarouselElementContainer.Data>
    {
        public readonly struct Data
        {
            public readonly bool IsScrollable;
            public readonly float ElementWidth;
            public readonly float ElementHeight;

            public Data(bool isScrollable, float elementWidth, float elementHeight)
            {
                IsScrollable = isScrollable;
                ElementWidth = elementWidth;
                ElementHeight = elementHeight;
            }
        }

        [Tooltip("Where should elements be anchored to if they can all be visible at the same time?")]
        [SerializeField] private TextAnchor _noScrollElementAlignment = TextAnchor.MiddleCenter;

        private RectTransform _rectTransform;
        private GridLayoutGroup _gridLayoutGroup;
        private ContentSizeFitter _contentSizeFitter;

        private bool _isScrollable = true;
        private float _elementWidth = 0f;
        private float _elementHeight = 0f;

        public void PrepareInitializationData(Data data)
        {
            _isScrollable = data.IsScrollable;
            _elementWidth = data.ElementWidth;
            _elementHeight = data.ElementHeight;
        }

        protected override void OnInitialize()
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
            _gridLayoutGroup.cellSize = new Vector2(_elementWidth, _elementHeight);

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

        public void SetElementSize(float width, float height)
        {
            _gridLayoutGroup.cellSize = new Vector2(width, height);
        }
    }
}