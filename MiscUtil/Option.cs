#if NET6_0_OR_GREATER

namespace MiscUtil;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public readonly struct Option<T> where T : notnull
{
    public static Option<T> None => default;
    public static Option<T> Some(T value) => new(value);

    private readonly bool _isSome;
    private readonly T _value;

    private Option(T value)
    {
        _value = value;
        _isSome = value is not null;
    }

    public bool IsNone => !_isSome;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isSome;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _isSome
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T>.Enumerator GetEnumerator()
    {
        return AsSpan().GetEnumerator();
    }
}

public static class Option
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) where T : notnull => Option<T>.Some(value);

#if NET7_0_OR_GREATER

    public static Option<T> Parse<T>(string s, IFormatProvider? provider = null)
        where T : IParsable<T>
    {
        return T.TryParse(s, provider, out var value)
            ? Option<T>.Some(value)
            : Option<T>.None;
    }

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
    public static U Match<T, U>(this Option<T> option,
                                Func<T, U> onSome,
                                Func<U> onNone)
        where T : notnull
    {
        return option.IsSome(out var value) ? onSome(value) : onNone();
    }

    public static Option<U> Bind<T, U>(this Option<T> option, Func<T, Option<U>> binder)
        where T : notnull where U : notnull
    {
        return option.IsSome(out var value)
            ? binder(value)
            : Option<U>.None;
    }

    public static Option<U> Map<T, U>(this Option<T> option, Func<T, U> mapper)
        where T : notnull where U : notnull
    {
        return option.IsSome(out var value)
            ? Option<U>.Some(mapper(value))
            : Option<U>.None;
    }
}

#endif