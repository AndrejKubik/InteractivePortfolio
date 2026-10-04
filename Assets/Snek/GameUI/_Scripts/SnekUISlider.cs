using Snek.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Snek.GameUI
{
    [UseSnekInspector]
    [RequireComponent(typeof(Slider))]
    public abstract class SnekUISlider : SnekMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected Slider _slider;

        private SnekUISliderHandle _handle;

        public SnekSliderData SetupData;

        [Space(10f)]
        public bool UseDragThreshold = false;

        [Range(1f, 100f)]
        public float DragThresholdPercent = 10f;

        [Space(10f)]
        public bool UseDragAreas = false;

        [Min(2)]
        public int DragAreaCount = 2;

        private SnekUISliderDragThresholdManager _dragThresholdManager;
        private SnekUISliderDragAreaManager _dragAreaManager;

        public bool IsHandleHeld { get; private set; }

        protected override void OnInitialize()
        {
            GetEssentialComponent(out _slider);
            GetEssentialComponent(out _handle, SnekGetComponentContext.Children);
        }

        protected override void Validate()
        {
            ValidateEssentialComponent(_slider, nameof(_slider));
            ValidateEssentialComponent(_handle, nameof(_handle));
        }

        protected override void OnInitializationSuccess()
        {
            _slider.wholeNumbers = SetupData.UseWholeNumbers;
            _slider.minValue = SetupData.MinValue;
            _slider.maxValue = SetupData.MaxValue;

            _slider.onValueChanged.AddListener(OnSliderMoveInternal);
            
            _handle.SetUserActionCallbacks(
                OnHandleGrabInternal,
                OnHandleDragInternal,
                OnHandleReleaseInternal);

            if (UseDragThreshold)
                _dragThresholdManager = new SnekUISliderDragThresholdManager(
                    _slider,
                    DragThresholdPercent,
                    OnDragThresholdReach);

            if (UseDragAreas)
                _dragAreaManager = new SnekUISliderDragAreaManager(_slider, DragAreaCount, OnDragAreaChange);
        }

        protected override void OnDispose()
        {
            _slider.onValueChanged.RemoveListener(OnSliderMoveInternal);
        }

        private void OnSliderMoveInternal(float newValue)
        {
            OnSliderMove(newValue);

            if (!IsHandleHeld)
                OnHandleGrabBase();

            if (UseDragThreshold && IsHandleHeld)
                _dragThresholdManager.OnSliderMove();

            if (UseDragAreas && IsHandleHeld)
                _dragAreaManager.OnSliderMove();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnHandleGrabInternal(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnHandleReleaseInternal(eventData);
        }

        private void OnHandleGrabInternal(PointerEventData eventData)
        {
            _slider.OnPointerDown(eventData);

            OnHandleGrabBase();
            OnHandleGrab();
        }

        private void OnHandleGrabBase()
        {
            IsHandleHeld = true;

            if (UseDragThreshold)
                _dragThresholdManager.OnHandleGrab();

            if (UseDragAreas)
                _dragAreaManager.OnHandleGrab();
        }

        private void OnHandleDragInternal(PointerEventData eventData)
        {
            _slider.OnDrag(eventData);
        }

        private void OnHandleReleaseInternal(PointerEventData eventData)
        {
            _slider.OnPointerUp(eventData);

            IsHandleHeld = false;

            OnHandleRelease();
        }

        public void SetValue(float newValue, bool notifySliderMove = true)
        {
            if(!_isValid)
            {
                Debug.LogError("Slider not initialized successfully, cannot set new value.");

                return;
            }

            if (notifySliderMove)
                _slider.value = newValue;
            else
                _slider.SetValueWithoutNotify(newValue);
        }

        public float GetValue()
        {
            return _slider.value;
        }

        protected virtual void OnSliderMove(float newValue) { }

        protected virtual void OnHandleGrab() { }

        protected virtual void OnDragThresholdReach() { }

        protected virtual void OnDragAreaChange() { }

        protected virtual void OnHandleRelease() { }
    }
}
