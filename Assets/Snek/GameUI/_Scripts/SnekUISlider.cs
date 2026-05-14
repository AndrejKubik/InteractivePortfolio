using Snek.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Snek.GameUI
{
    [UseSnekInspector]
    [RequireComponent(typeof(Slider))]
    public abstract class SnekUISlider : SnekMonoBehaviour
    {
        public Slider Slider { get; private set; }

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

        protected override void Initialize()
        {
            Slider = GetComponent<Slider>();
            _handle = GetComponentInChildren<SnekUISliderHandle>(true);
        }

        protected override void Validate()
        {
            if (!Slider)
                FailValidation("Cannot find Slider component.");

            if (!_handle)
                FailValidation("Cannot find SnekUISliderHandle component.");
        }

        protected override void OnInitializationSuccess()
        {
            Slider.wholeNumbers = SetupData.UseWholeNumbers;
            Slider.minValue = SetupData.MinValue;
            Slider.maxValue = SetupData.MaxValue;

            Slider.onValueChanged.AddListener(OnSliderMoveInternal);

            _handle.SetUserActionCallbacks(
                OnHandleGrabInternal,
                OnHandleDragInternal,
                OnHandleReleaseInternal);

            if (UseDragThreshold)
                _dragThresholdManager = new SnekUISliderDragThresholdManager(Slider, DragThresholdPercent, OnDragThresholdReach);

            if (UseDragAreas)
                _dragAreaManager = new SnekUISliderDragAreaManager(Slider, DragAreaCount, OnDragAreaChange);
        }

        protected virtual void OnDestroy()
        {
            Slider.onValueChanged.RemoveListener(OnSliderMove);
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

        protected virtual void OnSliderMove(float newValue)
        {

        }

        protected virtual void OnHandleGrab()
        {

        }

        protected virtual void OnDragThresholdReach()
        {

        }

        protected virtual void OnDragAreaChange()
        {

        }

        protected virtual void OnHandleRelease()
        {

        }

        private void OnHandleGrabInternal(PointerEventData eventData)
        {
            Slider.OnPointerDown(eventData);

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
            Slider.OnDrag(eventData);
        }

        private void OnHandleReleaseInternal(PointerEventData eventData)
        {
            Slider.OnPointerUp(eventData);

            IsHandleHeld = false;

            OnHandleRelease();
        }
    }
}
