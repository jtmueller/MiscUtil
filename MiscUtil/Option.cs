namespace MiscUtil;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.ArgumentNullException;

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

public static class OptionExtensions
{
    /// <summary>
    /// Executes and returns the value from either the <paramref name="onSome"/>
    /// or <paramref name="onNone"/> function, depending on the state of the option.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the match functions.</typeparam>
    /// <param name="self">The option to match on.</param>
    /// <param name="onSome">The match function that will be executed for a <c>Some</c> value.</param>
    /// <param name="onNone">The match function that will be executed for a <c>None</c> value.</param>
    /// <returns>The value given by the chosen match function.</returns>
    public static U Match<T, U>(this Option<T> self, Func<T, U> onSome, Func<U> onNone)
        where T : notnull
    {
        return self.IsSome(out var value) ? onSome(value) : onNone();
    }

    /// <summary>
    /// If the option has a value, passes that value to the <paramref name="binder"/> function,
    /// returning the option returned from that function. Useful for chaining a series of operations
    /// that each might fail.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the binder functions.</typeparam>
    /// <param name="self">The option to bind.</param>
    /// <param name="binder">The function that will be run on the option's value, if there is one.</param>
    /// <returns>The return value of the binder function, or <c>None</c>.</returns>
    public static Option<U> Bind<T, U>(this Option<T> self, Func<T, Option<U>> binder)
        where T : notnull where U : notnull
    {
        return self.IsSome(out var value)
            ? binder(value)
            : Option<U>.None;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the binder functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static Option<U> Map<T, U>(this Option<T> self, Func<T, U> mapper)
        where T : notnull where U : notnull
    {
        ThrowIfNull(mapper);

        return self.IsSome(out var value)
            ? Option<U>.Some(mapper(value))
            : Option<U>.None;
    }

    /// <summary>
    /// Returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to <see cref="MapOr"/> are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElse"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="U">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static U MapOr<T, U>(this Option<T> self, Func<T, U> mapper, U defaultValue)
        where T : notnull where U : notnull
    {
        ThrowIfNull(mapper);
        return self.IsSome(out var value) ? mapper(value) : defaultValue;
    }

    /// <summary>
    /// Computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="U">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static U MapOrElse<T, U>(this Option<T> self, Func<T, U> mapper, Func<U> defaultFactory)
        where T : notnull where U : notnull
    {
        ThrowIfNull(mapper);
        ThrowIfNull(defaultFactory);

        return self.IsSome(out var value) ? mapper(value) : defaultFactory();
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value, or throws an <see cref="InvalidOperationException"/>
    /// if the value is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to unwrap.</param>
    /// <param name="message">The message for the exception that gets thrown if the option has no value.</param>
    /// <returns>The value contained in the option.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the option does not contain a value.</exception>
    public static T Expect<T>(this Option<T> self, string message)
        where T : notnull
    {
        return self.IsSome(out var value)
            ? value : throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value, or throws an <see cref="InvalidOperationException"/>
    /// with a generic message if the value is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to unwrap.</param>
    /// <returns>The value contained in the option.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the option does not contain a value.</exception>
    public static T Unwrap<T>(this Option<T> self)
        where T : notnull
    {
        return self.IsSome(out var value)
            ? value : throw new InvalidOperationException("The option was expected to contain a value, but did not.");
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value or a provided default.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to bind.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    public static T UnwrapOr<T>(this Option<T> self, T defaultValue)
        where T : notnull
    {
        return self.IsSome(out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value or computes a default
    /// using the provided <paramref name="defaultFactory"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to unwrap.</param>
    /// <param name="defaultFactory">A function that returns a default value to use if the option is <c>None</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="defaultFactory"/> is null.</exception>
    public static T UnwrapOrElse<T>(this Option<T> self, Func<T> defaultFactory)
        where T : notnull
    {
        ThrowIfNull(defaultFactory);
        return self.IsSome(out var value) ? value : defaultFactory();
    }

    /// <summary>
    /// Transforms the <see cref="Option{T}"/> into a <see cref="Result{T,TErr}"/>,
    /// mapping <c>Some</c> to <c>Ok</c> and <c>None</c> to <c>Err</c> using the provided
    /// <paramref name="error"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">The option to transform.</param>
    /// <param name="error">The error to use if the option is <c>None</c>.</param>
    /// <returns>A <see cref="Result{T,TErr}"/> that contains either the option's value, or the provided error.</returns>
    public static Result<T, TErr> OkOr<T, TErr>(this Option<T> self, TErr error)
        where T : notnull where TErr : notnull
    {
        return self.IsSome(out var value)
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
    /// <param name="self">The option to transform.</param>
    /// <param name="errorFactory">A function that creates an error object to be used if the option is <c>None</c>.</param>
    /// <returns>A <see cref="Result{T,TErr}"/> that contains either the option's value, or the provided error.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="errorFactory"/> is null.</exception>
    public static Result<T, TErr> OkOrElse<T, TErr>(this Option<T> self, Func<TErr> errorFactory)
        where T : notnull where TErr : notnull
    {
        ThrowIfNull(errorFactory);
        return self.IsSome(out var value)
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
    /// <param name="self">An option containing a result.</param>
    /// <returns>An equivalent result containing an option.</returns>
    public static Result<Option<T>, TErr> Transpose<T, TErr>(this Option<Result<T, TErr>> self)
        where T : notnull where TErr : notnull
    {
        if (self.IsSome(out var result))
        {
            return result.Match(
                onOk: val => Result<Option<T>, TErr>.Ok(Option<T>.Some(val)),
                onErr: Result<Option<T>, TErr>.Err
            );
        }

        return Result<Option<T>, TErr>.Ok(Option<T>.None);
    }

    /// <summary>
    /// Removes one level of nesting from nested options.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The nested option to flatten.</param>
    /// <returns>The given option with one level of nesting removed.</returns>
    public static Option<T> Flatten<T>(this Option<Option<T>> self)
        where T : notnull
    {
        return self.IsSome(out var nested) ? nested : Option<T>.None;
    }

    /// <summary>
    /// Returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="predicate"/>
    /// and returns <c>Some</c> if the predicated returns <c>true</c>, otherwise <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The option to check.</param>
    /// <param name="predicate">The function that determines if the value in the option is valid to return.</param>
    /// <returns><c>Some</c> if the option is <c>Some</c> and the predicate returns <c>true</c>, otherwise <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(predicate);

        if (self.IsSome(out var value) && predicate(value))
        {
            return self;
        }

        return Option<T>.None;
    }

    /// <summary>
    /// Zips this <c>Option</c> with another <c>Option</c>.
    /// <para>If this option is <c>Some(s)</c> and other is <c>Some(o)</c>, this method returns <c>Some((s, o))</c>. Otherwise, <c>None</c> is returned.</para>
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <returns>An option containing the values from both input options, if both have values. Otherwise, <c>None</c>.</returns>
    public static Option<(T, U)> Zip<T, U>(this Option<T> self, Option<U> other)
        where T : notnull where U : notnull
    {
        if (self.IsSome(out var x) && other.IsSome(out var y))
        {
            return Option<(T, U)>.Some((x, y));
        }

        return Option<(T, U)>.None;
    }

    /// <summary>
    /// Zips this <c>Option</c> and another <c>Option</c> with function <paramref name="zipper"/>.
    /// <para>If this option is <c>Some(s)</c> and other is <c>Some(o)</c>, this method returns <c>Some(zipper(s, o))</c>. Otherwise, <c>None</c> is returned.</para>
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <typeparam name="V">The type returned by the <paramref name="zipper"/> function.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <param name="zipper">A functon that combines values from the two options into a new type.</param>
    /// <returns>An option contianing the result of passing both values to the <paramref name="zipper"/> function, or <c>None</c>.</returns>
    public static Option<V> ZipWith<T, U, V>(this Option<T> self, Option<U> other, Func<T, U, V> zipper)
        where T : notnull where U : notnull where V : notnull
    {
        ThrowIfNull(zipper);

        if (self.IsSome(out var x) && other.IsSome(out var y))
        {
            return Option<V>.Some(zipper(x, y));
        }

        return Option<V>.None;
    }

    /// <summary>
    /// Returns <c>None</c> if <paramref name="self"/> is <c>None</c>, otherwise returns <paramref name="other"/>.
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    public static Option<U> And<T, U>(this Option<T> self, Option<U> other)
        where T : notnull where U : notnull
    {
        return self.IsNone ? Option<U>.None : other;
    }

    /// <summary>
    /// Returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    public static Option<U> AndThen<T, U>(this Option<T> self, Func<T, Option<U>> thenFn)
        where T : notnull where U : notnull
    {
        return self.IsSome(out var value) ? thenFn(value) : Option<U>.None;
    }
}


// TODO: useful methods from https://doc.rust-lang.org/std/option/index.html#extracting-the-contained-value
// and, or, xor

// Also a .Some() extension method on any type?
// Also support for some well-known types with TryGetValue-type methods?
