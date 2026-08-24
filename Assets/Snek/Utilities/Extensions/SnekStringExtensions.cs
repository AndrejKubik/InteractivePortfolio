using System.Text.RegularExpressions;
using UnityEngine;

namespace Snek.Utilities
{
    public static class SnekStringExtensions
    {
        /// <summary>
        /// Splits words and forces pascal case format
        /// </summary>
        public static string Nicify(this string text)
        {
            string splitWordsText = Regex.Replace(text, @"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");

            return splitWordsText.Length > 0
                ? char.ToUpperInvariant(splitWordsText[0]) + splitWordsText[1..]
                : splitWordsText;
        }

        public static bool IsEmptyOrDefault(this string text, string defaultText)
        {
            return string.IsNullOrWhiteSpace(text) 
                || string.Equals(
                    text,
                    defaultText,
                    System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns HTML-colored version of the string
        /// </summary>
        public static string SetColor(this string text, Color color)
        {
            string colorHex = ColorUtility.ToHtmlStringRGBA(color);

            return $"<color=#{colorHex}>{text}</color>";
        }
    }
}
