using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public static class SnekDOTweenExtensions
{
    public static Tween DOSnekAnchoredPosition(this RectTransform rectTransform, Vector2 targetValue, float duration, AnimationCurve curve)
    {
        Vector2 startValue = rectTransform.anchoredPosition;

        return DOTween.To(animateValue, 0f, 1f, duration);

        void animateValue(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startValue, targetValue, value);
        }
    }

    public static Tween DOSnekAnchorOffsetMin(this RectTransform rectTransform, Vector2 targetValue, float duration, AnimationCurve curve)
    {
        Vector2 startValue = rectTransform.offsetMin;

        return DOTween.To(animateValue, 0f, 1f, duration);

        void animateValue(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.offsetMin = Vector2.LerpUnclamped(startValue, targetValue, value);
        }
    }

    public static Tween DOSnekAnchorOffsetMax(this RectTransform rectTransform, Vector2 targetValue, float duration, AnimationCurve curve)
    {
        Vector2 startValue = rectTransform.offsetMax;

        return DOTween.To(animateValue, 0f, 1f, duration);

        void animateValue(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.offsetMax = Vector2.LerpUnclamped(startValue, targetValue, value);
        }
    }

    public static Tween DOSnekScale(this RectTransform rectTransform, Vector2 targetValue, float duration, AnimationCurve curve)
    {
        Vector2 startValue = rectTransform.localScale;

        return DOTween.To(animateValue, 0f, 1f, duration);

        void animateValue(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.localScale = Vector2.LerpUnclamped(startValue, targetValue, value);
        }
    }

    public static Tween DoSnekAnchorMin(this RectTransform rectTransform, Vector2 targetValue, float duration, AnimationCurve curve)
    {
        Vector2 startValue = rectTransform.anchorMin;

        return DOTween.To(animateValue, 0f, 1f, duration);

        void animateValue(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.anchorMin = Vector2.LerpUnclamped(startValue, targetValue, value);
        }
    }

    public static Tween DoSnekAnchorMax(this RectTransform rectTransform, Vector2 targetValue, float duration, AnimationCurve curve)
    {
        Vector2 startValue = rectTransform.anchorMax;

        return DOTween.To(animateValue, 0f, 1f, duration);

        void animateValue(float t)
        {
            float value = curve.Evaluate(t);

            rectTransform.anchorMax = Vector2.LerpUnclamped(startValue, targetValue, value);
        }
    }
}
