using UnityEngine;

namespace Snek.Utilities
{
    public static class SnekRectTransformExtensions
    {
        public static bool IsOverlapping(this RectTransform rectTransform, RectTransform otherRectTransform)
        {
            Rect worldRect = GetRectFromWorldCorners(rectTransform);
            Rect otherWorldRect = GetRectFromWorldCorners(otherRectTransform);

            return worldRect.Overlaps(otherWorldRect);
        }

        private static Rect GetRectFromWorldCorners(RectTransform transform)
        {
            Vector3[] worldCorners = new Vector3[4];

            transform.GetWorldCorners(worldCorners);

            return new Rect(
                worldCorners[0].x,
                worldCorners[0].y,
                worldCorners[2].x - worldCorners[0].x,
                worldCorners[2].y - worldCorners[0].y);
        }

        /// <summary>
        /// Resets pivot and anchor values to default centered values
        /// </summary>
        public static void ResetPivotAndAnchor(this RectTransform rectTransform)
        {
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.pivot = Vector2.one * 0.5f;
        }

        public static void SetAnchorOffset(this RectTransform rectTransform, float newValue, AnchorOffsetSide side)
        {
            switch (side)
            {
                case AnchorOffsetSide.Left:

                    rectTransform.offsetMin = new Vector2(newValue, rectTransform.offsetMin.y);
                    break;

                case AnchorOffsetSide.Right:

                    rectTransform.offsetMax = new Vector2(-newValue, rectTransform.offsetMax.y);
                    break;

                case AnchorOffsetSide.Top:

                    rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -newValue);
                    break;

                case AnchorOffsetSide.Bottom:

                    rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, newValue);
                    break;
            }
        }

        public static void ResetAnchorOffset(this RectTransform rectTransform)
        {
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
