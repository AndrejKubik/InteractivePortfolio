using UnityEngine;

namespace SnekEditor.GUIUtilities
{
    /// <summary>
    /// Creates a clone of the provided Rect which you can split into different Rects by consuming it's space
    /// </summary>
    public struct SnekRectSplitter
    {
        private readonly Rect _originalRect;

        private Rect _remainingRect;

        public SnekRectSplitter(Rect rect)
        {
            _originalRect = rect;
            _remainingRect = rect;
        }

        public Rect GetRemainingRect()
        {
            return _remainingRect;
        }

        public Rect TakeLeftPercentage(float percent, bool useOriginalSize = false)
        {
            percent = Mathf.Clamp01(percent);

            float sourceWidth = useOriginalSize ? _originalRect.width : _remainingRect.width;

            return TakeLeft(sourceWidth * percent);
        }

        public Rect TakeRightPercentage(float percent, bool useOriginalSize = false)
        {
            percent = Mathf.Clamp01(percent);

            float sourceWidth = useOriginalSize ? _originalRect.width : _remainingRect.width;

            return TakeRight(sourceWidth * percent);
        }

        public Rect TakeTopPercentage(float percent, bool useOriginalSize = false)
        {
            percent = Mathf.Clamp01(percent);

            float sourceHeight = useOriginalSize ? _originalRect.height : _remainingRect.height;

            return TakeTop(sourceHeight * percent);
        }

        public Rect TakeBottomPercentage(float percent, bool useOriginalSize = false)
        {
            percent = Mathf.Clamp01(percent);

            float sourceHeight = useOriginalSize ? _originalRect.height : _remainingRect.height;

            return TakeBottom(sourceHeight * percent);
        }

        public Rect TakeRemaining()
        {
            Rect remainingRect = _remainingRect;
            _remainingRect = Rect.zero;

            return remainingRect;
        }

        public Rect TakeLeft(float width)
        {
            ConsumeWidth(width, out float consumedWidth);

            var leftRect = new Rect(_remainingRect.x, _remainingRect.y, consumedWidth, _remainingRect.height);

            _remainingRect.x += consumedWidth;

            return leftRect;
        }

        public Rect TakeRight(float width)
        {
            ConsumeWidth(width, out float consumedWidth);

            var rightRect = new Rect(_remainingRect.xMax, _remainingRect.y, consumedWidth, _remainingRect.height);

            return rightRect;
        }

        public Rect TakeTop(float height)
        {
            ConsumeHeight(height, out float consumedHeight);

            var topRect = new Rect(_remainingRect.x, _remainingRect.y, _remainingRect.width, consumedHeight);

            _remainingRect.y += consumedHeight;

            return topRect;
        }

        public Rect TakeBottom(float height)
        {
            ConsumeHeight(height, out float consumedHeight);

            var bottomRect = new Rect(_remainingRect.x, _remainingRect.yMax, _remainingRect.width, consumedHeight);

            return bottomRect;
        }

        private void ConsumeWidth(float requestedWidth, out float consumedWidth)
        {
            requestedWidth = Mathf.Max(0f, requestedWidth);

            float availableWidth = _remainingRect.width;

            consumedWidth = Mathf.Min(requestedWidth, availableWidth);

            _remainingRect.width -= consumedWidth;
        }

        private void ConsumeHeight(float requestedHeight, out float consumedHeight)
        {
            requestedHeight = Mathf.Max(0f, requestedHeight);

            float availableHeight = _remainingRect.height;

            consumedHeight = Mathf.Min(requestedHeight, availableHeight);

            _remainingRect.height -= consumedHeight;
        }
    }
}
