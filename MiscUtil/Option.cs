namespace MiscUtil;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using static System.ArgumentNullException;
using System.Globalization;

/// <summary>
/// <see cref="Option{T}"/> represents an optional value: every <see cref="Option{T}"/> is either <c>Some</c> and contains a value, or <c>None</c>, and does not. 
/// </summary>
/// <typeparam name="T">The type the opton might contain.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>, ISpanFormattable
    where T : notnull
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

// TODO: useful methods from https://doc.rust-lang.org/std/option/index.html#extracting-the-contained-value
// map_or, and, or, xor, filter, zip, flatten

// Also a .Some() extension method on any type?
// Also support for some well-known types with TryGetValue-type methods?

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

    /// <summary>
    /// Returns the contained <c>Some</c> value, or throws an <see cref="InvalidOperationException"/>
    /// if the value is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="option">The option to unwrap.</param>
    /// <param name="message">The message for the exception that gets thrown if the option has no value.</param>
    /// <returns>The value contained in the option.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the option does not contain a value.</exception>
    public static T Expect<T>(this Option<T> option, string message)
        where T : notnull
    {
        return option.IsSome(out var value)
            ? value : throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value, or throws an <see cref="InvalidOperationException"/>
    /// with a generic message if the value is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="option">The option to unwrap.</param>
    /// <returns>The value contained in the option.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the option does not contain a value.</exception>
    public static T Unwrap<T>(this Option<T> option)
        where T : notnull
    {
        return option.IsSome(out var value)
            ? value : throw new InvalidOperationException("The option was expected to contain a value, but did not.");
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value or a provided default.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="option">The option to bind.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    public static T UnwrapOr<T>(this Option<T> option, T defaultValue)
        where T : notnull
    {
        return option.IsSome(out var value)
            ? value : defaultValue;
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value or computes a default
    /// using the provided <paramref name="defaultFactory"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="option">The option to unwrap.</param>
    /// <param name="defaultFactory">A function that returns a default value to use if the option is <c>None</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="defaultFactory"/> is null.</exception>
    public static T UnwrapOrElse<T>(this Option<T> option, Func<T> defaultFactory)
        where T : notnull
    {
        ThrowIfNull(defaultFactory);
        return option.IsSome(out var value)
            ? value : defaultFactory();
    }

    /// <summary>
    /// Transforms the <see cref="Option{T}"/> into a <see cref="Result{T,TErr}"/>,
    /// mapping <c>Some</c> to <c>Ok</c> and <c>None</c> to <c>Err</c> using the provided
    /// <paramref name="error"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="option">The option to transform.</param>
    /// <param name="error">The error to use if the option is <c>None</c>.</param>
    /// <returns>A <see cref="Result{T,TErr}"/> that contains either the option's value, or the provided error.</returns>
    public static Result<T, TErr> OkOr<T, TErr>(this Option<T> option, TErr error)
        where T : notnull where TErr : notnull
    {
        return option.IsSome(out var value)
            ? Result<T, TErr>.Ok(value)
            : Result<T, TErr>.Err(error);
    }

    /// <summary>
    /// Transforms the <see cref="Option{T}"/> into a <see cref="Result{T,TErr}"/>,
    /// mapping <c>Some</c> to <c>Ok</c> and <c>None</c> to <c>Err</c> using the provided
    /// <paramref name="errorFactory"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="option">The option to transform.</param>
    /// <param name="errorFactory">A function that creates an error object to be used if the option is <c>None</c>.</param>
    /// <returns>A <see cref="Result{T,TErr}"/> that contains either the option's value, or the provided error.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="errorFactory"/> is null.</exception>
    public static Result<T, TErr> OkOrElse<T, TErr>(this Option<T> option, Func<TErr> errorFactory)
        where T : notnull where TErr : notnull
    {
        ThrowIfNull(errorFactory);
        return option.IsSome(out var value)
            ? Result<T, TErr>.Ok(value)
            : Result<T, TErr>.Err(errorFactory());
    }

    /// <summary>
    /// Transposes an <c>Option</c> of a <c>Result</c> into a <c>Result</c> of an <c>Option</c>.
    /// <para>
    ///     <c>None</c> will be mapped to <c>Ok(None)</c>. 
    ///     <c>Some(Ok(_))</c> and <c>Some(Err(_))</c> will be mapped to <c>Ok(Some(_))</c> and <c>Err(_)</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="option">An option containing a result.</param>
    /// <returns>An equivalent result containing an option.</returns>
    public static Result<Option<T>, TErr> Transpose<T, TErr>(this Option<Result<T, TErr>> option)
        where T : notnull where TErr : notnull
    {
        if (option.IsSome(out var result))
        {
            return result.Match(
                onOk: val => Result<Option<T>, TErr>.Ok(Option<T>.Some(val)),
                onErr: Result<Option<T>, TErr>.Err
            );
        }

        return Result<Option<T>, TErr>.Ok(Option<T>.None);
    }
}
