using System;
#if NETSTANDARD2_0
using System.Buffers;
using System.Buffers.Text;
using System.Text;
#else
using System.Collections.Generic;
using System.Globalization;
#endif

namespace MiscUtil
{
    public static class SpanExtensions
    {
#if NETSTANDARD2_0
        private const int s_maxStack = 256;

        /// <summary>
        ///     Fast integer-based power function. Use instead of Math.Pow for integers.
        /// </summary>
        private static int Pow(int x, int exp)
        {
            switch (exp)
            {
                case 0: return 1;
                case 1: return x;
                case 2: return x * x;
                case 3: return x * x * x;
                case 4: return x * x * x * x;
                case 5: return x * x * x * x * x;
                case 6: return x * x * x * x * x * x;
                case 7: return x * x * x * x * x * x * x;
                case 8: return x * x * x * x * x * x * x * x;
                case 9: return x * x * x * x * x * x * x * x * x;
                case 10: return x * x * x * x * x * x * x * x * x * x;
                case 11: return x * x * x * x * x * x * x * x * x * x * x;
                case 12: return x * x * x * x * x * x * x * x * x * x * x * x;
                default:
                    int ret = 1;
                    while (exp != 0)
                    {
                        if ((exp & 1) == 1)
                            ret *= x;
                        x *= x;
                        exp >>= 1;
                    }
                    return ret;
            }
        }
#endif

        /// <summary>
        ///     Parses an integer from a char span consisting only of numeric characters, prefixed by an optional minus sign.
        /// </summary>
#if NETSTANDARD2_0
        public static int? ToInt32(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            bool isNegative = source[0] == '-';
            if (isNegative)
                source = source.Slice(1);
            int sum = 0;

            for (int i = 0, len = source.Length; i < len; i++)
            {
                char cur = source[i];
                if (cur < '0' || cur > '9')
                    return null;

                int pow = len - 1 - i;
                sum += ((cur - '0') * Pow(10, pow));
            }

            return isNegative ? sum * -1 : sum;
        }
#else
        public static int? ToInt32(this ReadOnlySpan<char> source)
        {
            if (int.TryParse(source, out int parsed))
                return parsed;
            return null;
        }
#endif

        /// <summary>
        ///     Parses a double from a char span consisting only of numeric characters, prefixed by an optional minus sign.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe double? ToDouble(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[]? pooledBytes = null;
            try
            {
                int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
                if (maxByteCount > s_maxStack)
                    pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

                int encodedByteCount;
                fixed (char* cp = source)
                fixed (byte* bp = bytes)
                {
                    encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                }

                if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out double value, out int bytesConsumed))
                {
                    // If we didn't consume all the bytes, it should fail to parse.
                    if (bytesConsumed < encodedByteCount)
                        return null;
                    return value;
                }
            }
            finally
            {
                if (pooledBytes != null)
                    ArrayPool<byte>.Shared.Return(pooledBytes);
            }

            return null;
        }
#else
        public static double? ToDouble(this ReadOnlySpan<char> source)
        {
            if (double.TryParse(source, out double parsed))
                return parsed;
            return null;
        }
#endif

        /// <summary>
        ///     Parses a decimal from a char span consisting only of numeric characters, prefixed by an optional minus sign.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe decimal? ToDecimal(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[]? pooledBytes = null;
            try
            {
                int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
                if (maxByteCount > s_maxStack)
                    pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

                int encodedByteCount;
                fixed (char* cp = source)
                fixed (byte* bp = bytes)
                {
                    encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                }

                if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out decimal value, out int bytesConsumed))
                {
                    // If we didn't consume all the bytes, it should fail to parse.
                    if (bytesConsumed < encodedByteCount)
                        return null;
                    return value;
                }
            }
            finally
            {
                if (pooledBytes != null)
                    ArrayPool<byte>.Shared.Return(pooledBytes);
            }

            return null;
        }
#else
        public static decimal? ToDecimal(this ReadOnlySpan<char> source)
        {
            if (decimal.TryParse(source, out decimal parsed))
                return parsed;
            return null;
        }
#endif

        /// <summary>
        ///     Parses a <see cref="Guid"/> from a char span.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe Guid? ToGuid(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            char formatChar = source.Length switch
            {
                36 => 'D',
                32 => 'N',
                38 when source[0] == '{' => 'B',
                38 when source[0] == '(' => 'P',
                68 => 'X',
                _ => '\0'
            };

            byte[]? pooledBytes = null;
            try
            {
                int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
                if (maxByteCount > s_maxStack)
                    pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

                int encodedByteCount;
                fixed (char* cp = source)
                fixed (byte* bp = bytes)
                {
                    encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                }

                if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out Guid value, out _, formatChar))
                {
                    return value;
                }
            }
            finally
            {
                if (pooledBytes != null)
                    ArrayPool<byte>.Shared.Return(pooledBytes);
            }

            return null;
        }
#else
        public static Guid? ToGuid(this ReadOnlySpan<char> source)
        {
            if (Guid.TryParse(source, out var parsed))
                return parsed;
            return null;
        }
#endif

#if NETSTANDARD2_0
        /// <summary>
        ///     Parses a <see cref="DateTime"/> from a char span. Limited format support: Only supports the G, O, and R DateTime formats.
        /// </summary>
        /// <param name="source">The span to parse.</param>
        /// <param name="formatChar">The optional format specifier to use for parsing. Only the following formats are supported: G, O, R.</param>
        public static unsafe DateTime? ToDateTime(this ReadOnlySpan<char> source, char formatChar = '\0')
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[]? pooledBytes = null;
            try
            {
                int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
                if (maxByteCount > s_maxStack)
                    pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

                int encodedByteCount;
                fixed (char* cp = source)
                fixed (byte* bp = bytes)
                {
                    encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                }

                if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out DateTime value, out _, formatChar))
                {
                    return value;
                }
            }
            finally
            {
                if (pooledBytes != null)
                    ArrayPool<byte>.Shared.Return(pooledBytes);
            }

            return null;
        }
#else

        static readonly Lazy<Dictionary<char, char[]>> s_formatChars = new Lazy<Dictionary<char, char[]>>(() =>
        {
            var chars = new[] { 'd', 'f', 'g', 'm', 'o', 'r', 's', 't', 'u', 'y' };
            var dict = new Dictionary<char, char[]>(chars.Length * 2);
            foreach (var c in chars)
            {
                dict.Add(c, new[] { c });
                dict.Add(char.ToUpperInvariant(c), new[] { char.ToUpperInvariant(c) });
            }
            return dict;
        });

        /// <summary>
        ///     Parses a <see cref="DateTime"/> from a char span.
        /// </summary>
        /// <param name="source">The span to parse.</param>
        /// <param name="format">The optional format specifier to use for parsing.</param>
        public static DateTime? ToDateTime(this ReadOnlySpan<char> source, char formatChar = '\0')
        {
            // Utf8Parser only allows the invariant culture, so we use it here to match behavior
            if (formatChar == '\0')
            {
                if (DateTime.TryParse(source, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
                {
                    return parsed;
                }
            }
            else
            {
                var formatChars = s_formatChars.Value.TryGetValue(formatChar, out var chars)
                    ? chars.AsSpan() : formatChar.ToString().AsSpan();
                if (DateTime.TryParseExact(source, formatChars, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
                {
                    return parsed;
                }
            }
            return null;
        }

        /// <summary>
        ///     Parses a <see cref="DateTime"/> from a char span.
        /// </summary>
        /// <param name="source">The span to parse.</param>
        /// <param name="format">The optional format specifier to use for parsing.</param>
        public static DateTime? ToDateTime(this ReadOnlySpan<char> source, ReadOnlySpan<char> format)
        {
            if (DateTime.TryParseExact(source, format, CultureInfo.InvariantCulture, 
                DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
            {
                return parsed;
            }
            return null;
        }
#endif

#if NETSTANDARD2_0
        /// <summary>
        ///     Parses a <see cref="DateTimeOffset"/> from a char span. Limited format support: Only supports the G and R DateTimeOffset formats.
        /// </summary>
        /// <param name="source">The span to parse.</param>
        /// <param name="format">The optional format specifier to use for parsing. Only the following formats are supported: G, R.</param>
        public static unsafe DateTimeOffset? ToDateTimeOffset(this ReadOnlySpan<char> source, char formatChar = '\0')
        {
            if (source.IsEmpty) return null;

            byte[]? pooledBytes = null;
            try
            {
                int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
                if (maxByteCount > s_maxStack)
                    pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

                int encodedByteCount;
                fixed (char* cp = source)
                fixed (byte* bp = bytes)
                {
                    encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                }

                if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out DateTimeOffset value, out _, formatChar))
                {
                    return value;
                }
            }
            finally
            {
                if (pooledBytes != null)
                    ArrayPool<byte>.Shared.Return(pooledBytes);
            }

            return null;
        }
#else
        /// <summary>
        ///     Parses a <see cref="DateTimeOffset"/> from a char span.
        /// </summary>
        /// <param name="source">The span to parse.</param>
        /// <param name="format">The optional format specifier to use for parsing.</param>
        public static DateTimeOffset? ToDateTimeOffset(this ReadOnlySpan<char> source, char formatChar = '\0')
        {
            // Utf8Parser only allows the invariant culture, so we use it here to match behavior
            if (formatChar == '\0')
            {
                if (DateTimeOffset.TryParse(source, CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
                {
                    return parsed;
                }
            }
            else
            {
                var formatChars = s_formatChars.Value.TryGetValue(formatChar, out var chars)
                    ? chars.AsSpan() : formatChar.ToString().AsSpan();
                if (DateTimeOffset.TryParseExact(source, formatChars, CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
                {
                    return parsed;
                }
            }
            return null;
        }

        /// <summary>
        ///     Parses a <see cref="DateTimeOffset"/> from a char span.
        /// </summary>
        /// <param name="source">The span to parse.</param>
        /// <param name="format">The optional format specifier to use for parsing.</param>
        public static DateTimeOffset? ToDateTimeOffset(this ReadOnlySpan<char> source, ReadOnlySpan<char> format)
        {
            if (DateTimeOffset.TryParseExact(source, format, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
            {
                return parsed;
            }
            return null;
        }
#endif

        /// <summary>
        ///     Parses a boolean from a char span.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe bool? ToBoolean(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[]? pooledBytes = null;
            try
            {
                int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
                if (maxByteCount > s_maxStack)
                    pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

                int encodedByteCount;
                fixed (char* cp = source)
                fixed (byte* bp = bytes)
                {
                    encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                }

                if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out bool value, out int bytesConsumed))
                {
                    // If we didn't consume all the bytes, it should fail to parse.
                    if (bytesConsumed < encodedByteCount)
                        return null;
                    return value;
                }
            }
            finally
            {
                if (pooledBytes != null)
                    ArrayPool<byte>.Shared.Return(pooledBytes);
            }

            return null;
        }
#else
        public static bool? ToBoolean(this ReadOnlySpan<char> source)
        {
            if (bool.TryParse(source, out bool parsed))
                return parsed;
            return null;
        }
#endif

        /// <summary>
        ///     Efficiently determines without allocations if two strings are equal after they are both trimmed.
        /// </summary>
        public static bool TrimEquals(this string source, string other, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return source.AsSpan().Trim().Equals(other.AsSpan().Trim(), comparisonType);
        }

        /// <summary>
        ///     Determines if two char spans are equal after they are both trimmed.
        /// </summary>
        public static bool TrimEquals(this ReadOnlySpan<char> source, ReadOnlySpan<char> other, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return source.Trim().Equals(other.Trim(), comparisonType);
        }
    }
}
