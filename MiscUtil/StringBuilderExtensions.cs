using System;
using System.Globalization;
using System.Text;

namespace MiscUtil
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        ///     Removes all trailing occurrences of whitespace characters from the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb)
            => TrimEnd(sb, ReadOnlySpan<char>.Empty);

        /// <summary>
        ///     Removes all trailing occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[]? trimChars)
            => TrimEnd(sb, trimChars);

        /// <summary>
        ///     Removes all trailing occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, ReadOnlySpan<char> trimChars)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            if (sb.Length == 0)
                return sb;

            int length = sb.Length;

            if (trimChars.IsEmpty)
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
                    if (trimChars.IndexOf(sb[i]) > -1)
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
        ///     Removes all leading occurrences of whitespace characters from the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimStart(this StringBuilder sb)
            => TrimStart(sb, ReadOnlySpan<char>.Empty);

        /// <summary>
        ///     Removes all leading occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[]? trimChars)
            => TrimStart(sb, trimChars);

        /// <summary>
        ///     Removes all leading occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes trailing whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder TrimStart(this StringBuilder sb, ReadOnlySpan<char> trimChars)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));

            if (sb.Length == 0)
                return sb;

            int removeCount = 0;

            if (trimChars.IsEmpty)
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
                    if (trimChars.IndexOf(sb[i]) > -1)
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
        ///     Removes all leading and trailing occurrences of whitespace characters from the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder Trim(this StringBuilder sb)
            => sb.TrimEnd(ReadOnlySpan<char>.Empty).TrimStart(ReadOnlySpan<char>.Empty);

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
        ///     Removes all leading and trailing occurrences of a set of characters from the current <see cref="StringBuilder"/> object.
        ///     If no characters are specified, removes whitespace.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="trimChars">An array of Unicode characters to remove.</param>
        /// <returns>The <see cref="StringBuilder"/> instance with the indicated trailing characters removed.</returns>
        public static StringBuilder Trim(this StringBuilder sb, ReadOnlySpan<char> trimChars)
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
            if (chars.IsEmpty)
                return sb;

            int startIndex = sb.Length;
            sb.Length += chars.Length;
            for (int i = 0, len = chars.Length; i < len; i++)
            {
                sb[startIndex + i] = chars[i];
            }

            return sb;
        }

        /// <summary>
        /// Copies the characters from a specified segment of this instance to the destination span.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/></param>
        /// <param name="sourceIndex">The index within the StringBuilder at which to start counting.</param>
        /// <param name="destination">The span to copy characters into.</param>
        /// <param name="count">The number of characters to copy.</param>
        public static void CopyTo(this StringBuilder sb, int sourceIndex, Span<char> destination, int count)
        {
            if (sb is null)
                throw new ArgumentNullException(nameof(sb));
            if ((uint)sourceIndex >= (uint)sb.Length)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            if (sourceIndex + count > sb.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count > destination.Length)
                throw new ArgumentException("Destination too small.");
            
            for (int i = 0; i < count; i++)
            {
                destination[i] = sb[sourceIndex + i];
            }
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

#if NETSTANDARD2_1
        /// <summary>
        ///     Removes the given <see cref="Range"/> of characters from the StringBuilder.
        /// </summary>
        public static StringBuilder Remove(this StringBuilder sb, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(sb.Length);
            sb.Remove(offset, length);
            return sb;
        }
#endif
    }
}
