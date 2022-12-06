using System.Globalization;

namespace MiscUtil;

public static class SpanExtensions
{
    /// <summary>
    ///     Parses an integer from a char span consisting only of numeric characters, prefixed by an optional minus sign.
    /// </summary>
    public static int? ToInt32(this ReadOnlySpan<char> source)
    {
        if (int.TryParse(source, out int parsed))
            return parsed;
        return null;
    }

    /// <summary>
    ///     Parses an integer from a string consisting only of numeric characters, prefixed by an optional minus sign.
    /// </summary>
    public static int? ToInt32(this string source)
        => ToInt32(source.AsSpan());

    /// <summary>
    ///     Parses a long from a char span.
    /// </summary>
    public static long? ToInt64(this ReadOnlySpan<char> source)
    {
        if (long.TryParse(source, out long parsed))
            return parsed;
        return null;
    }

    /// <summary>
    ///     Parses a long from a string.
    /// </summary>
    public static long? ToInt64(this string source)
        => ToInt64(source.AsSpan());

    /// <summary>
    ///     Parses a float from a char span.
    /// </summary>
    public static float? ToFloat(this ReadOnlySpan<char> source)
    {
        if (float.TryParse(source, out float parsed))
            return parsed;
        return null;
    }

    /// <summary>
    ///     Parses a float from a string.
    /// </summary>
    public static float? ToFloat(this string source)
        => ToFloat(source.AsSpan());

    /// <summary>
    ///     Parses a double from a char span.
    /// </summary>
    public static double? ToDouble(this ReadOnlySpan<char> source)
    {
        if (double.TryParse(source, out double parsed))
            return parsed;
        return null;
    }

    /// <summary>
    ///     Parses a double from a string.
    /// </summary>
    public static double? ToDouble(this string source)
        => ToDouble(source.AsSpan());

    /// <summary>
    ///     Parses a decimal from a char span.
    /// </summary>
    public static decimal? ToDecimal(this ReadOnlySpan<char> source)
    {
        if (decimal.TryParse(source, out decimal parsed))
            return parsed;
        return null;
    }

    /// <summary>
    ///     Parses a decimal from a string.
    /// </summary>
    public static decimal? ToDecimal(this string source)
        => ToDecimal(source.AsSpan());

    /// <summary>
    ///     Parses a <see cref="Guid"/> from a char span.
    /// </summary>
    public static Guid? ToGuid(this ReadOnlySpan<char> source)
    {
        if (Guid.TryParse(source, out var parsed))
            return parsed;
        return null;
    }

    /// <summary>
    ///     Parses a <see cref="Guid"/> from a string.
    /// </summary>
    public static Guid? ToGuid(this string source)
        => ToGuid(source.AsSpan());

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

    /// <summary>
    ///     Parses a boolean from a char span.
    /// </summary>
    public static bool? ToBoolean(this ReadOnlySpan<char> source)
    {
        if (bool.TryParse(source, out bool parsed))
            return parsed;
        return null;
    }

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
