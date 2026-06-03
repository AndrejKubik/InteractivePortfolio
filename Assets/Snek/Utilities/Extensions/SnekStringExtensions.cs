using System.Text.RegularExpressions;

namespace Snek.Utilities
{
    public static class SnekStringExtensions
    {
        /// <summary>
        /// Splits words from camel case and pascal case format
        /// </summary>
        public static string Nicify(this string text)
        {
            return Regex.Replace(text, @"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
        }
    }
}
