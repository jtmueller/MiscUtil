#if NETSTANDARD2_0
using System.Buffers;
using System.Buffers.Text;
using System.Text;
#else
using System.Globalization;
#endif

namespace MiscUtil;

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
                {
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
            source = source[1..];
        int sum = 0;

        for (int i = 0, len = source.Length; i < len; i++)
        {
            char cur = source[i];
            if (cur is < '0' or > '9')
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
    ///     Parses an integer from a string consisting only of numeric characters, prefixed by an optional minus sign.
    /// </summary>
    public static int? ToInt32(this string source)
        => ToInt32(source.AsSpan());

    /// <summary>
    ///     Parses a long from a char span.
    /// </summary>
#if NETSTANDARD2_0
    public static unsafe long? ToInt64(this ReadOnlySpan<char> source)
    {
        source = source.Trim();
        if (source.IsEmpty) return null;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out long value, out int bytesConsumed))
            {
                // If we didn't consume all the bytes, it should fail to parse.
                if (bytesConsumed < encodedByteCount)
                    return null;
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
                ArrayPool<byte>.Shared.Return(pooledBytes);
        }

        return null;
    }
#else
    public static long? ToInt64(this ReadOnlySpan<char> source)
    {
        if (long.TryParse(source, out long parsed))
            return parsed;
        return null;
    }
#endif

    /// <summary>
    ///     Parses a long from a string.
    /// </summary>
    public static long? ToInt64(this string source)
        => ToInt64(source.AsSpan());

    /// <summary>
    ///     Parses a float from a char span.
    /// </summary>
#if NETSTANDARD2_0
    public static unsafe float? ToFloat(this ReadOnlySpan<char> source)
    {
        source = source.Trim();
        if (source.IsEmpty) return null;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out float value, out int bytesConsumed))
            {
                // If we didn't consume all the bytes, it should fail to parse.
                if (bytesConsumed < encodedByteCount)
                    return null;
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
                ArrayPool<byte>.Shared.Return(pooledBytes);
        }

        return null;
    }
#else
    public static float? ToFloat(this ReadOnlySpan<char> source)
    {
        if (float.TryParse(source, out float parsed))
            return parsed;
        return null;
    }
#endif

    /// <summary>
    ///     Parses a float from a string.
    /// </summary>
    public static float? ToFloat(this string source)
        => ToFloat(source.AsSpan());

    /// <summary>
    ///     Parses a double from a char span.
    /// </summary>
#if NETSTANDARD2_0
    public static unsafe double? ToDouble(this ReadOnlySpan<char> source)
    {
        source = source.Trim();
        if (source.IsEmpty) return null;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out double value, out int bytesConsumed))
            {
                // If we didn't consume all the bytes, it should fail to parse.
                if (bytesConsumed < encodedByteCount)
                    return null;
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
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
    ///     Parses a double from a string.
    /// </summary>
    public static double? ToDouble(this string source)
        => ToDouble(source.AsSpan());

    /// <summary>
    ///     Parses a decimal from a char span.
    /// </summary>
#if NETSTANDARD2_0
    public static unsafe decimal? ToDecimal(this ReadOnlySpan<char> source)
    {
        source = source.Trim();
        if (source.IsEmpty) return null;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out decimal value, out int bytesConsumed))
            {
                // If we didn't consume all the bytes, it should fail to parse.
                if (bytesConsumed < encodedByteCount)
                    return null;
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
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
    ///     Parses a decimal from a string.
    /// </summary>
    public static decimal? ToDecimal(this string source)
        => ToDecimal(source.AsSpan());

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

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out Guid value, out _, formatChar))
            {
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
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

    /// <summary>
    ///     Parses a <see cref="Guid"/> from a string.
    /// </summary>
    public static Guid? ToGuid(this string source)
        => ToGuid(source.AsSpan());

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

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out DateTime value, out _, formatChar))
            {
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
                ArrayPool<byte>.Shared.Return(pooledBytes);
        }

        return null;
    }

    /// <summary>
    ///     Parses a <see cref="DateTime"/> from a string. Limited format support: Only supports the G, O, and R DateTime formats.
    /// </summary>
    /// <param name="source">The span to parse.</param>
    /// <param name="formatChar">The optional format specifier to use for parsing. Only the following formats are supported: G, O, R.</param>
    public static DateTime? ToDateTime(this string source, char formatChar = '\0')
        => ToDateTime(source.AsSpan(), formatChar);
#else

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
            Span<char> formatChars = stackalloc char[1];
            formatChars[0] = formatChar;
            if (DateTime.TryParseExact(source, formatChars, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
            {
                return parsed;
            }
        }
        return null;
    }

    /// <summary>
    ///     Parses a <see cref="DateTime"/> from a string.
    /// </summary>
    /// <param name="source">The span to parse.</param>
    /// <param name="format">The optional format specifier to use for parsing.</param>
    public static DateTime? ToDateTime(this string source, char formatChar = '\0')
        => ToDateTime(source.AsSpan(), formatChar);

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

    /// <summary>
    ///     Parses a <see cref="DateTime"/> from a string.
    /// </summary>
    /// <param name="source">The span to parse.</param>
    /// <param name="format">The optional format specifier to use for parsing.</param>
    public static DateTime? ToDateTime(this string source, ReadOnlySpan<char> format)
        => ToDateTime(source.AsSpan(), format);
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

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out DateTimeOffset value, out _, formatChar))
            {
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
                ArrayPool<byte>.Shared.Return(pooledBytes);
        }

        return null;
    }

    /// <summary>
    ///     Parses a <see cref="DateTimeOffset"/> from a string. Limited format support: Only supports the G and R DateTimeOffset formats.
    /// </summary>
    /// <param name="source">The span to parse.</param>
    /// <param name="format">The optional format specifier to use for parsing. Only the following formats are supported: G, R.</param>
    public static DateTimeOffset? ToDateTimeOffset(this string source, char formatChar = '\0')
        => ToDateTimeOffset(source.AsSpan(), formatChar);

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
            Span<char> formatChars = stackalloc char[1];
            formatChars[0] = formatChar;
            if (DateTimeOffset.TryParseExact(source, formatChars, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
            {
                return parsed;
            }
        }
        return null;
    }

    /// <summary>
    ///     Parses a <see cref="DateTimeOffset"/> from a string.
    /// </summary>
    /// <param name="source">The span to parse.</param>
    /// <param name="format">The optional format specifier to use for parsing.</param>
    public static DateTimeOffset? ToDateTimeOffset(this string source, char formatChar = '\0')
        => ToDateTimeOffset(source.AsSpan(), formatChar);

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

    /// <summary>
    ///     Parses a <see cref="DateTimeOffset"/> from a string.
    /// </summary>
    /// <param name="source">The span to parse.</param>
    /// <param name="format">The optional format specifier to use for parsing.</param>
    public static DateTimeOffset? ToDateTimeOffset(this string source, ReadOnlySpan<char> format)
        => ToDateTimeOffset(source.AsSpan(), format);

#endif

    /// <summary>
    ///     Parses a boolean from a char span.
    /// </summary>
#if NETSTANDARD2_0
    public static unsafe bool? ToBoolean(this ReadOnlySpan<char> source)
    {
        source = source.Trim();
        if (source.IsEmpty) return null;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(source.Length);
        byte[]? pooledBytes = maxByteCount > s_maxStack ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        try
        {
            Span<byte> bytes = pooledBytes ?? stackalloc byte[maxByteCount];

            int encodedByteCount;
            fixed (char* cp = source)
            fixed (byte* bp = bytes)
            {
                encodedByteCount = Encoding.UTF8.GetBytes(cp, source.Length, bp, maxByteCount);
            }

            if (Utf8Parser.TryParse(bytes[..encodedByteCount], out bool value, out int bytesConsumed))
            {
                // If we didn't consume all the bytes, it should fail to parse.
                if (bytesConsumed < encodedByteCount)
                    return null;
                return value;
            }
        }
        finally
        {
            if (pooledBytes is not null)
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
    ///     Parses a boolean from a string.
    /// </summary>
    public static bool? ToBoolean(this string source)
        => ToBoolean(source.AsSpan());

    /// <summary>
    ///     Efficiently determines without allocations if two strings are equal after they are both trimmed.
    /// </summary>
    public static bool TrimEquals(this string source, string other, StringComparison comparisonType = StringComparison.Ordinal)
        => source.AsSpan().Trim().Equals(other.AsSpan().Trim(), comparisonType);

    /// <summary>
    ///     Determines if two char spans are equal after they are both trimmed.
    /// </summary>
    public static bool TrimEquals(this ReadOnlySpan<char> source, ReadOnlySpan<char> other, StringComparison comparisonType = StringComparison.Ordinal)
        => source.Trim().Equals(other.Trim(), comparisonType);
}
