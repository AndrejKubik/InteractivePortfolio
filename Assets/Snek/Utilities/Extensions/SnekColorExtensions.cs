using UnityEngine;

namespace Snek.Utilities
{
    public static class SnekColorExtensions
    {
        /// <summary>
        /// Returns the exact same color value with custom alpha value
        /// </summary>
        public static Color ChangeAlpha(this Color color, float newAlpha)
        {
            if(newAlpha < 0f || newAlpha > 1f)
            {
                Debug.LogError("Invalid alpha value provided, cannot change color's alpha.");

                return color;
            }

            color.a = newAlpha;

            return color;
        }
    }
}
