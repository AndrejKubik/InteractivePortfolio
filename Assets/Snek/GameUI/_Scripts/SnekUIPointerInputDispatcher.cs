using Snek.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Snek.GameUI
{
    [UseSnekInspector]
    [RequireComponent(typeof(RectTransform))]
    public class SnekUIPointerInputDispatcher : SnekMonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public bool IsDragging { get; private set; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
        }
    }
}
