using System;
using Snek.Utilities;
using UnityEngine.EventSystems;

namespace Snek.GameUI
{
    [UseSnekInspector]
    public class SnekUISliderHandle : SnekMonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        private Action<PointerEventData> _onGrabCallback;
        private Action<PointerEventData> _onDragCallback;
        private Action<PointerEventData> _onReleaseCallback;

        public void SetUserActionCallbacks(
            Action<PointerEventData> onGrabCallback,
            Action<PointerEventData> onDragCallback,
            Action<PointerEventData> onReleaseCallback)
        {
            _onGrabCallback = onGrabCallback;
            _onDragCallback = onDragCallback;
            _onReleaseCallback = onReleaseCallback;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onGrabCallback?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _onDragCallback?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _onReleaseCallback?.Invoke(eventData);
        }
    }
}
