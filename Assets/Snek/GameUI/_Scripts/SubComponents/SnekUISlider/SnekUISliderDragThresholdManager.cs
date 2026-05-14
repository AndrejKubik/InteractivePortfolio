using System;
using UnityEngine;
using UnityEngine.UI;

namespace Snek.GameUI
{
    [Serializable]
    /// <summary>
    /// Runtime data container for slider drag threshold reached callback
    /// </summary>
    public class SnekUISliderDragThresholdManager
    {
        private readonly Slider _slider;
        private readonly float _dragThreshold = 0.1f;
        private readonly Action _onThresholdReachedCallback;

        private float _previousValue;

        public SnekUISliderDragThresholdManager(Slider slider, float dragThresholdPercent, Action onThresholdReachedCallback)
        {
            dragThresholdPercent = Mathf.Clamp(dragThresholdPercent, 1f, 100f);

            _slider = slider;
            _dragThreshold = Mathf.Lerp(slider.minValue, slider.maxValue, dragThresholdPercent * 0.01f);
            _onThresholdReachedCallback = onThresholdReachedCallback;
        }

        public void OnSliderMove()
        {
            if (!IsValueInNewArea())
                return;

            _previousValue = _slider.value;

            _onThresholdReachedCallback.Invoke();
        }

        public void OnHandleGrab()
        {
            _previousValue = _slider.value;
        }

        private bool IsValueInNewArea()
        {
            float valueDifference = Mathf.Abs(_slider.value - _previousValue);

            return valueDifference >= _dragThreshold;
        }
    }
}
