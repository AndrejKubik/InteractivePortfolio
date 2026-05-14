using System;
using UnityEngine;
using UnityEngine.UI;

namespace Snek.GameUI
{
    [Serializable]
    public class SnekUISliderDragAreaManager
    {
        private readonly Slider _slider;
        private readonly Action _onAreaChangedCallback;

        private SnekUiSliderDragArea[] _sliderAreas;
        private SnekUiSliderDragArea _currentArea;

        public SnekUISliderDragAreaManager(Slider slider, int totalAreas, Action onAreaChangedCallback)
        {
            _slider = slider;
            _onAreaChangedCallback = onAreaChangedCallback;

            float sliderRange = slider.maxValue - slider.minValue; //this is for sliders that have min value other than 0f
            float areaSize = sliderRange / totalAreas;

            GenerateAreas(totalAreas, areaSize);
        }

        private void GenerateAreas(int totalAreas, float areaSize)
        {
            _sliderAreas = new SnekUiSliderDragArea[totalAreas];

            for (int i = 0; i < totalAreas; i++)
            {
                float minValue = areaSize * i;
                float maxValue = areaSize * (i + 1);

                _sliderAreas[i] = new SnekUiSliderDragArea(minValue, maxValue);
            }
        }

        public void OnHandleGrab()
        {
            SetCurrentArea(GetAreaForCurrentValue());
        }

        public void OnSliderMove()
        {
            if (IsSliderInArea(_currentArea))
                return;

            SetCurrentArea(GetAreaForCurrentValue());

            _onAreaChangedCallback.Invoke();
        }

        private SnekUiSliderDragArea? GetAreaForCurrentValue()
        {
            foreach (SnekUiSliderDragArea area in _sliderAreas)
                if(IsSliderInArea(area))
                    return area;

            Debug.Log(_slider.value);
            return null;
        }

        private bool IsSliderInArea(SnekUiSliderDragArea area)
        {
            return area.MinValue <= _slider.value && _slider.value <= area.MaxValue;
        }

        private void SetCurrentArea(SnekUiSliderDragArea? newArea)
        {
            if (!newArea.HasValue)
            {
                Debug.LogError(
                    $"Failed to find slider area.\n" +
                    $"Slider Value: {_slider.value}");

                return;
            }

            _currentArea = newArea.Value;
        }
    }
}
