using System;
using System.Globalization;
using System.Text;

namespace MiscUtil
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        ///     Removes all trailing occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

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

        /// <summary>
        ///     Removes all leading occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[] trimChars)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            if (sb.Length == 0)
                return sb;

            int removeCount = 0;

            if (trimChars is null || trimChars.Length == 0)
            {
                // trim whitespace
                for (int i = 0; i < sb.Length; i++)
                {
                    if (char.IsWhiteSpace(sb[i]))
                        removeCount++;
                    else
                        break;
                }
            }
            else
            {
                // trim specific chars
                for (int i = 0; i < sb.Length; i++)
                {
                    if (Array.IndexOf(trimChars, sb[i]) > -1)
                        removeCount++;
                    else
                        break;
                }
            }

            if (removeCount != 0)
                sb.Remove(0, removeCount);

            return sb;
        }

        /// <summary>
        ///     Removes all leading and trailing occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder Trim(this StringBuilder sb, params char[] trimChars)
            => sb.TrimEnd(trimChars).TrimStart(trimChars);

        /// <summary>
        /// Converts all the characters in the <see cref="StringBuilder"/> to upper-case, using the given culture.
        /// </summary>
        public static StringBuilder ToUpper(this StringBuilder sb, CultureInfo culture)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToUpper(sb[i], culture);
            }

            return sb;
        }

        /// <summary>
        /// Converts all the characters in the <see cref="StringBuilder"/> to upper-case, using the invariant culture.
        /// </summary>
        public static StringBuilder ToUpperInvariant(this StringBuilder sb)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToUpperInvariant(sb[i]);
            }

            return sb;
        }

        /// <summary>
        /// Converts all the characters in the <see cref="StringBuilder"/> to lower-case, using the given culture.
        /// </summary>
        public static StringBuilder ToLower(this StringBuilder sb, CultureInfo culture)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToLower(sb[i], culture);
            }

            return sb;
        }

        /// <summary>
        /// Converts all the characters in the <see cref="StringBuilder"/> to lower-case, using the invariant culture.
        /// </summary>
        public static StringBuilder ToLowerInvariant(this StringBuilder sb)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));
            
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToLowerInvariant(sb[i]);
            }

            return sb;
        }

#if NETSTANDARD2_0
        /// <summary>
        ///     Appends the characters in the specified <see cref="ReadOnlySpan{char}"/> to this instance.
        /// </summary>
        public static StringBuilder Append(this StringBuilder sb, ReadOnlySpan<char> chars)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            int startIndex = sb.Length;
            sb.Length += chars.Length;
            for (int i = 0; i < chars.Length; i++)
            {
                sb[startIndex + i] = chars[i];
            }

            return sb;
        }
#endif

        /// <summary>
        ///     Appends the characters in the specified <see cref="ReadOnlyMemory{char}"/> to this instance.
        /// </summary>
        /// <remarks>
        ///     This overload prevents accidentally sending a ReadOnlyMemory to the Object overload of Append,
        ///     which would call ToString and appear to work, but at greatly reduced efficiency.
        /// </remarks>
        public static StringBuilder Append(this StringBuilder sb, ReadOnlyMemory<char> chars)
            => sb.Append(chars.Span);
    }
}
