using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public static class SnekDOTweenExtensions
{
    public static Tween DOResetAnchorOffsetAndPosition(this RectTransform rectTransform, float duration, AnimationCurve curve)
    {
        rectTransform.DOKill();

        Vector2 startOffsetMin = rectTransform.offsetMin;
        Vector2 startOffsetMax = rectTransform.offsetMax;
        Vector2 startPosition = rectTransform.anchoredPosition;

        return DOTween.To(animateValues, 0f, 1f, duration);

        void animateValues(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.offsetMin = Vector2.LerpUnclamped(startOffsetMin, Vector2.zero, value);
            rectTransform.offsetMax = Vector2.LerpUnclamped(startOffsetMax, Vector2.zero, value);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPosition, Vector2.zero, value);
        }
    }
}
