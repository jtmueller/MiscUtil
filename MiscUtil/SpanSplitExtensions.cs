// Borrowed from here until .net 5
// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/SpanSplitEnumerator.T.cs

namespace MiscUtil;

public static class SpanSplitExtensions
{
    /// <summary>
    /// Returns a type that allows for enumeration of each element within a split span
    /// using a single space as a separator character.
    /// </summary>
    /// <param name="span">The source span to be enumerated.</param>
    /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
    public static SpanSplitEnumerator<char> Split(this ReadOnlySpan<char> span) => new(span, ' ');

    /// <summary>
    /// Returns a type that allows for enumeration of each element within a split span
    /// using the provided separator character.
    /// </summary>
    /// <param name="span">The source span to be enumerated.</param>
    /// <param name="separator">The separator value to be used to split the provided span.</param>
    /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
    public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>
        => new(span, separator);

    /// <summary>
    /// Returns a type that allows for enumeration of each element within a split span
    /// using the provided separator string.
    /// </summary>
    /// <param name="span">The source span to be enumerated.</param>
    /// <param name="separator">The separator string to be used to split the provided span.</param>
    /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
    public static SpanSplitEnumerator<char> Split(this ReadOnlySpan<char> span, string separator)
        => new(span, separator.AsSpan());

    /// <summary>
    /// Returns a type that allows for enumeration of each element within a split span
    /// using the provided separator string.
    /// </summary>
    /// <param name="span">The source span to be enumerated.</param>
    /// <param name="separator">The separator sequence to be used to split the provided span.</param>
    /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
    public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator) where T : IEquatable<T>
        => new(span, separator);

    /// <summary>
    /// Returns the first two results from splitting a span on a given separator, as <see cref="Range"/> objects that can be used
    /// to slice the original array. If fewer than two results are found, resulting ranges may be empty.
    /// </summary>
    /// <param name="span">The source span to split.</param>
    /// <param name="separator">The separator value to be used to split the provided span.</param>
    /// <returns></returns>
    public static (Range, Range) Split2<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>
    {
        SpanSplitEnumerator<T> enumerator = new(span, separator);
        var firstPart = ^0..;

        if (enumerator.MoveNext())
        {
            firstPart = enumerator.Current;
        }

        if (enumerator.MoveNext())
        {
            return (firstPart, enumerator.Current);
        }

        return (firstPart, ^0..);
    }

    /// <summary>
    /// Returns the first two results from splitting a span on a given separator, as <see cref="Range"/> objects that can be used
    /// to slice the original array. If fewer than two results are found, resulting ranges may be empty.
    /// </summary>
    /// <param name="span">The source span to split.</param>
    /// <param name="separator">The separator sequence to be used to split the provided span.</param>
    /// <returns></returns>
    public static (Range, Range) Split2<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator) where T : IEquatable<T>
    {
        SpanSplitEnumerator<T> enumerator = new(span, separator);
        var firstPart = ^0..;

        if (enumerator.MoveNext())
        {
            firstPart = enumerator.Current;
        }

        if (enumerator.MoveNext())
        {
            return (firstPart, enumerator.Current);
        }

        return (firstPart, ^0..);
    }

    /// <summary>
    /// Returns the first three results from splitting a span on a given separator, as <see cref="Range"/> objects that can be used
    /// to slice the original array. If fewer than two results are found, resulting ranges may be empty.
    /// </summary>
    /// <param name="span">The source span to split.</param>
    /// <param name="separator">The separator value to be used to split the provided span.</param>
    /// <returns></returns>
    public static (Range, Range, Range) Split3<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>
    {
        SpanSplitEnumerator<T> enumerator = new(span, separator);
        var firstPart = ^0..;
        var secondPart = ^0..;

        if (enumerator.MoveNext())
        {
            firstPart = enumerator.Current;
        }

        if (enumerator.MoveNext())
        {
            secondPart = enumerator.Current;
        }

        if (enumerator.MoveNext())
        {
            return (firstPart, secondPart, enumerator.Current);
        }

        return (firstPart, secondPart, ^0..);
    }

    /// <summary>
    /// Returns the first three results from splitting a span on a given separator, as <see cref="Range"/> objects that can be used
    /// to slice the original array. If fewer than two results are found, resulting ranges may be empty.
    /// </summary>
    /// <param name="span">The source span to split.</param>
    /// <param name="separator">The separator sequence to be used to split the provided span.</param>
    /// <returns></returns>
    public static (Range, Range, Range) Split3<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator) where T : IEquatable<T>
    {
        SpanSplitEnumerator<T> enumerator = new(span, separator);
        var firstPart = ^0..;
        var secondPart = ^0..;

        if (enumerator.MoveNext())
        {
            firstPart = enumerator.Current;
        }

        if (enumerator.MoveNext())
        {
            secondPart = enumerator.Current;
        }

        if (enumerator.MoveNext())
        {
            return (firstPart, secondPart, enumerator.Current);
        }

        return (firstPart, secondPart, ^0..);
    }
}

/// <summary>
/// <see cref="SpanSplitEnumerator{T}"/> allows for enumeration of each element within a <see cref="ReadOnlySpan{T}"/>
/// that has been split using a provided separator.
/// </summary>
public ref struct SpanSplitEnumerator<T> where T : IEquatable<T>
{
    private readonly ReadOnlySpan<T> _buffer;

    private readonly ReadOnlySpan<T> _separators;
    private readonly T _separator;

    private readonly int _separatorLength;
    private readonly bool _splitOnSingleToken;

    private readonly bool _isInitialized;

    private int _startCurrent;
    private int _endCurrent;
    private int _startNext;

    /// <summary>
    /// Returns an enumerator that allows for iteration over the split span.
    /// </summary>
    /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/> that can be used to iterate over the split span.</returns>
    public SpanSplitEnumerator<T> GetEnumerator() => this;

    /// <summary>
    /// Returns the current element of the enumeration.
    /// </summary>
    /// <returns>Returns a <see cref="Range"/> instance that indicates the bounds of the current element withing the source span.</returns>
    public Range Current => new(_startCurrent, _endCurrent);

    internal SpanSplitEnumerator(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
    {
        _isInitialized = true;
        _buffer = span;
        _separators = separators;
        _separator = default!;
        _splitOnSingleToken = false;
        _separatorLength = _separators.Length != 0 ? _separators.Length : 1;
        _startCurrent = 0;
        _endCurrent = 0;
        _startNext = 0;
    }

    internal SpanSplitEnumerator(ReadOnlySpan<T> span, T separator)
    {
        _isInitialized = true;
        _buffer = span;
        _separator = separator;
        _separators = default;
        _splitOnSingleToken = true;
        _separatorLength = 1;
        _startCurrent = 0;
        _endCurrent = 0;
        _startNext = 0;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the enumeration.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the enumeration.</returns>
    public bool MoveNext()
    {
        if (!_isInitialized || _startNext > _buffer.Length)
        {
            return false;
        }

        var slice = _buffer.Slice(_startNext);
        _startCurrent = _startNext;

        int separatorIndex = _splitOnSingleToken ? slice.IndexOf(_separator) : slice.IndexOf(_separators);
        int elementLength = separatorIndex != -1 ? separatorIndex : slice.Length;

        _endCurrent = _startCurrent + elementLength;
        _startNext = _endCurrent + _separatorLength;
        return true;
    }
}
