namespace MiscUtil;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>
/// <see cref="Option{T}"/> represents an optional value: every <see cref="Option{T}"/> is either <c>Some</c> and contains a value, or <c>None</c>, and does not. 
/// </summary>
/// <typeparam name="T">The type the opton might contain.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>, ISpanFormattable
    where T : notnull
{
    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    public static Option<T> None
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/>.
    /// <para>NOTE: Nulls are not allowed; a null value will result in a <c>None</c> option even when calling <see cref="Some"/>.</para>
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some(T value) => new(value);

    private readonly bool _isSome;
    private readonly T _value;

    private Option(T value)
    {
        _value = value;
        _isSome = value is not null;
    }

    /// <summary>
    /// Returns <c>true</c> if the option is <c>None</c>.
    /// </summary>
    public bool IsNone => !_isSome;

    /// <summary>
    /// Returns <c>true</c> if the option is <c>Some</c>, and returns the contained
    /// value through <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value contained in the option.</param>
    /// <returns><c>true</c> if the option is <c>Some</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isSome;
    }

    /// <summary>
    /// Converts the option into a <see cref="ReadOnlySpan{T}"/> that contains either zero or one
    /// items depending on whether the option is <c>Some</c> or <c>None</c>.
    /// </summary>
    /// <returns>A span containing the option's value, or an empty span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _isSome
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    /// <summary>
    /// Returns an enumerator that allows the option to be iterated with a <c>foreach</c> loop.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T>.Enumerator GetEnumerator()
    {
        return AsSpan().GetEnumerator();
    }

    /// <inheritdoc />
    public bool Equals(Option<T> other)
    {
        if (_isSome != other._isSome)
            return false;

        if (!_isSome)
            return true;

        if (_value is IEquatable<T> eq)
            return eq.Equals(other._value);

        return _value.Equals(other._value);
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Option<T> opt && Equals(opt);

    /// <inheritdoc />
    public override int GetHashCode() => _isSome ? _value.GetHashCode() : 0;

    /// <inheritdoc />
    public override string ToString()
    {
        return _isSome 
            ? string.Create(CultureInfo.InvariantCulture, $"Some({_value})") 
            : "None";
    }

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (_isSome)
        {
            if (_value is ISpanFormattable fmt)
            {
                if ("Some(".AsSpan().TryCopyTo(destination))
                {
                    var remainingDest = destination[5..];
                    if (fmt.TryFormat(remainingDest, out var innerWritten, format, provider))
                    {
                        remainingDest = remainingDest[innerWritten..];
                        if (remainingDest.Length >= 1)
                        {
                            remainingDest[0] = ')';
                            charsWritten = innerWritten + 6;
                            return true;
                        }
                    }
                }
                else
                {
                    var output = format.IsEmpty
                        ? string.Create(provider, $"Some({_value})")
                        : string.Format(provider, $"Some({{0:{format}}})", _value);

                    if (output.AsSpan().TryCopyTo(destination))
                    {
                        charsWritten = output.Length;
                        return true;
                    }
                }
            }

            charsWritten = 0;
            return false;
        }

        if ("None".AsSpan().TryCopyTo(destination))
        {
            charsWritten = 4;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return _isSome
            ? string.IsNullOrEmpty(format)
                ? string.Create(formatProvider, $"Some({_value})")
                : string.Format(formatProvider, "Some({0:" + format + "})", _value)
            : "None";
    }

    /// <inheritdoc/>
    public int CompareTo(Option<T> other)
    {
        return (_isSome, other._isSome) switch
        {
            (true, true) => Comparer<T>.Default.Compare(_value, other._value),
            (true, false) => 1,
            (false, true) => -1,
            (false, false) => 0
        };
    }

    /// <inheritdoc />
    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator >(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <inheritdoc />
    public static bool operator <(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <inheritdoc />
    public static bool operator >=(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) >= 0;
    }

    /// <inheritdoc />
    public static bool operator <=(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) <= 0;
    }
}

/// <summary>
/// <see cref="Option"/> represents an optional value: every <see cref="Option{T}"/> is either <c>Some</c> and contains a value, or <c>None</c>, and does not. 
/// </summary>
/// <typeparam name="T">The type the opton might contain.</typeparam>
public static class Option
{
    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) where T : notnull
        => Option<T>.Some(value);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> if it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Create<T>(T? value) where T : class
        => Option<T>.Some(value!);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> is it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Create<T>(T? value)
        where T : struct
    {
        return value.HasValue
            ? Option<T>.Some(value.GetValueOrDefault())
            : Option<T>.None;
    }

    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() where T : notnull => default;

#if NET7_0_OR_GREATER

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(string s, IFormatProvider? provider)
        where T : IParsable<T>
    {
        return T.TryParse(s, provider, out var value)
            ? Option<T>.Some(value)
            : Option<T>.None;
    }

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(string s) where T : IParsable<T>
        => Parse<T>(s, provider: null);

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(ReadOnlySpan<char> s, IFormatProvider? provider)
        where T : ISpanParsable<T>
    {
        return T.TryParse(s, provider, out var value)
            ? Option<T>.Some(value)
            : Option<T>.None;
    }

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(ReadOnlySpan<char> s) where T : ISpanParsable<T>
        => Parse<T>(s, provider: null);

#endif
}
