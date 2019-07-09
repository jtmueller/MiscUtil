using System;
using System.Text;

namespace MiscUtil
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        ///     Removes all trailing occurrences of a set of characters specified in an array from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            if (sb is null)
                throw new NullReferenceException();

            if (sb.Length == 0)
                return sb;

            int length = sb.Length;

            if (trimChars is null || trimChars.Length == 0)
            {
                // trim whitespace
                for (int i = sb.Length - 1; i >= 0; i--)
                {
                    if (char.IsWhiteSpace(sb[i]))
                        length--;
                    else
                        break;
                }
            }
            else
            {
                // trim specific chars
                for (int i = sb.Length - 1; i >= 0; i--)
                {
                    if (Array.IndexOf(trimChars, sb[i]) > -1)
                        length--;
                    else
                        break;
                }
            }

            if (length < sb.Length)
                sb.Length = length;

            return sb;
        }
    }
}
