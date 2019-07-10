using System;
#if NETSTANDARD2_0
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
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
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        ///     Does not support globalization. In .NET Core or a hypothetical future version of .NET Framework with built-in
        ///     support for parsing char spans, this method will be obsolete.
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
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe double? ToDouble(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[] pooledBytes = null;
            try
            {
                fixed (char* cp = source)
                {
                    int maxByteCount = Encoding.UTF8.GetByteCount(cp, source.Length);
                    if (maxByteCount > s_maxStack)
                        pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                    Span<byte> bytes = maxByteCount > s_maxStack ? pooledBytes : stackalloc byte[maxByteCount];

                    fixed (byte* bp = bytes)
                    {
                        int encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                        if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out double value, out int bytesConsumed))
                        {
                            // If we didn't consume all the bytes, it should fail to parse.
                            if (bytesConsumed < encodedByteCount)
                                return null;
                            return value;
                        }
                    }
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
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe decimal? ToDecimal(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[] pooledBytes = null;
            try
            {
                fixed (char* cp = source)
                {
                    int maxByteCount = Encoding.UTF8.GetByteCount(cp, source.Length);
                    if (maxByteCount > s_maxStack)
                        pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                    Span<byte> bytes = maxByteCount > s_maxStack ? pooledBytes : stackalloc byte[maxByteCount];

                    fixed (byte* bp = bytes)
                    {
                        int encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                        if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out decimal value, out int bytesConsumed))
                        {
                            // If we didn't consume all the bytes, it should fail to parse.
                            if (bytesConsumed < encodedByteCount)
                                return null;
                            return value;
                        }
                    }
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
        ///     Parses a GUID from a char span.
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
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

            byte[] pooledBytes = null;
            try
            {
                fixed (char* cp = source)
                {
                    int maxByteCount = Encoding.UTF8.GetByteCount(cp, source.Length);
                    if (maxByteCount > s_maxStack)
                        pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                    Span<byte> bytes = maxByteCount > s_maxStack ? pooledBytes : stackalloc byte[maxByteCount];

                    fixed (byte* bp = bytes)
                    {
                        int encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                        if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out Guid value, out int bytesConsumed, formatChar))
                        {
                            // If we didn't consume all the bytes, it should fail to parse.
                            if (bytesConsumed < encodedByteCount)
                                return null;
                            return value;
                        }
                    }
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

        /* REMOVED: DateTime parsing using Utf8Parser doesn't seem to work at the moment. 
         * Commenting out so nobody uses these currently-broken methods by mistake.

        /// <summary>
        ///     Parses a DateTime from a char span.
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        /// </summary>
        /// <param name="source"></param>
        public static DateTime? ToDateTime(this ReadOnlySpan<char> source)
        {
            if (source.IsEmpty) return null;

            char[] chars = null;
            byte[] bytes = null;
            try
            {
                // This part would be more efficient in .NET Core as it could get bytes directly from the span
                // without first copying the span to a char array.
                chars = ArrayPool<char>.Shared.Rent(source.Length);
                source.CopyTo(chars);
                var maxByteCount = Encoding.UTF8.GetByteCount(chars, 0, source.Length);
                bytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                var encodedByteCount = Encoding.UTF8.GetBytes(chars, 0, source.Length, bytes, 0);

                if (Utf8Parser.TryParse(bytes.AsSpan(0, encodedByteCount), out DateTime value, out int bytesConsumed))
                {
                    return value;
                }

                return null;
            }
            finally
            {
                if (chars != null)
                    ArrayPool<char>.Shared.Return(chars);
                if (bytes != null)
                    ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        /// <summary>
        ///     Parses a DateTimeOffset from a char span.
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        /// </summary>
        /// <param name="source"></param>
        public static DateTimeOffset? ToDateTimeOffset(this ReadOnlySpan<char> source)
        {
            if (source.IsEmpty) return null;

            char[] chars = null;
            byte[] bytes = null;
            try
            {
                // This part would be more efficient in .NET Core as it could get bytes directly from the span
                // without first copying the span to a char array.
                chars = ArrayPool<char>.Shared.Rent(source.Length);
                source.CopyTo(chars);
                var maxByteCount = Encoding.UTF8.GetByteCount(chars, 0, source.Length);
                bytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                var encodedByteCount = Encoding.UTF8.GetBytes(chars, 0, source.Length, bytes, 0);

                if (Utf8Parser.TryParse(bytes.AsSpan(0, encodedByteCount), out DateTimeOffset value, out int bytesConsumed))
                {
                    return value;
                }

                return null;
            }
            finally
            {
                if (chars != null)
                    ArrayPool<char>.Shared.Return(chars);
                if (bytes != null)
                    ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        */

        /// <summary>
        ///     Parses a boolean from a char span.
        ///     Useful when parsing integers from a substring, as span slicing avoids creating new strings just to parse them.
        /// </summary>
#if NETSTANDARD2_0
        public static unsafe bool? ToBoolean(this ReadOnlySpan<char> source)
        {
            source = source.Trim();
            if (source.IsEmpty) return null;

            byte[] pooledBytes = null;
            try
            {
                fixed (char* cp = source)
                {
                    int maxByteCount = Encoding.UTF8.GetByteCount(cp, source.Length);
                    if (maxByteCount > s_maxStack)
                        pooledBytes = ArrayPool<byte>.Shared.Rent(maxByteCount);
                    Span<byte> bytes = maxByteCount > s_maxStack ? pooledBytes : stackalloc byte[maxByteCount];

                    fixed (byte* bp = bytes)
                    {
                        int encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
                        if (Utf8Parser.TryParse(bytes.Slice(0, encodedByteCount), out bool value, out int bytesConsumed))
                        {
                            // If we didn't consume all the bytes, it should fail to parse.
                            if (bytesConsumed < encodedByteCount)
                                return null;
                            return value;
                        }
                    }
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
