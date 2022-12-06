namespace MiscUtil;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

/// <summary>
/// <see cref="Option{T}"/> represents an optional value: every <see cref="Option{T}"/> is either <c>Some</c> and contains a value, or <c>None</c>, and does not. 
/// </summary>
/// <typeparam name="T">The type the opton might contain.</typeparam>
public readonly struct Option<T> where T : notnull
{
    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    public static Option<T> None => default;

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
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
    /// <returns>Returns <c>true</c> if the option is <c>Some</c>.</returns>
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
    public static Option<T> Some<T>(T value) where T : notnull => Option<T>.Some(value);

#if NET7_0_OR_GREATER

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(string s, IFormatProvider? provider = null)
        where T : IParsable<T>
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
    public static Option<T> Parse<T>(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        where T : ISpanParsable<T>
    {
        return T.TryParse(s, provider, out var value)
            ? Option<T>.Some(value)
            : Option<T>.None;
    }

#endif
}

// TODO: useful methods from https://doc.rust-lang.org/std/option/enum.Option.html
// expect, map_or, ok_or, and, or, xor, filter, zip, transpose, flatten

// Also a .Some() extension method on any type?
// Also a "bind to try method" option like Genix?

public static class OptionExtensions
{
    /// <summary>
    /// Executes and returns the value from either the <paramref name="onSome"/>
    /// or <paramref name="onNone"/> function, depending on the state of the option.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the match functions.</typeparam>
    /// <param name="option">The option to match on.</param>
    /// <param name="onSome">The match function that will be executed for a <c>Some</c> value.</param>
    /// <param name="onNone">The match function that will be executed for a <c>None</c> value.</param>
    /// <returns>Returns the value given by the chosen match function.</returns>
    public static U Match<T, U>(this Option<T> option,
                                Func<T, U> onSome,
                                Func<U> onNone)
        where T : notnull
    {
        return option.IsSome(out var value) ? onSome(value) : onNone();
    }

    /// <summary>
    /// If the option has a value, passes that value to the <paramref name="binder"/> function,
    /// returning the option returned from that function. Useful for chaining a series of operations
    /// that each might fail.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the binder functions.</typeparam>
    /// <param name="option">The option to bind.</param>
    /// <param name="binder">The function that will be run on the option's value, if there is one.</param>
    /// <returns>The return value of the binder function, or <c>None</c>.</returns>
    public static Option<U> Bind<T, U>(this Option<T> option, Func<T, Option<U>> binder)
        where T : notnull where U : notnull
    {
        return option.IsSome(out var value)
            ? binder(value)
            : Option<U>.None;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the binder functions.</typeparam>
    /// <param name="option">The option to bind.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    public static Option<U> Map<T, U>(this Option<T> option, Func<T, U> mapper)
        where T : notnull where U : notnull
    {
        return option.IsSome(out var value)
            ? Option<U>.Some(mapper(value))
            : Option<U>.None;
    }
}
