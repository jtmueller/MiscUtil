using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.ArgumentNullException;

namespace MiscUtil;

// TODO: implement ISpanFormattable
// TODO: [StructLayout(LayoutKind.Explicit)] to get err/value fields to inhabit the same space? Need unit tests first.

public readonly struct Result<T, TErr> : IEquatable<Result<T, TErr>>, IComparable<Result<T, TErr>>
    where T : notnull where TErr : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Ok(T value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Err(TErr error) => new(error);

    private Result(T value)
    {
        ThrowIfNull(value);
        _value = value;
        _err = default!;
        _isOk = true;
    }

    private Result(TErr error)
    {
        ThrowIfNull(error);
        _err = error;
        _value = default!;
        _isOk = false;
    }

    private readonly bool _isOk;
    private readonly T _value;
    private readonly TErr _err;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErr([MaybeNullWhen(false)] out TErr error)
    {
        error = _err;
        return !_isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Match<U>(Func<T, U> onOk, Func<TErr, U> onErr)
    {
        ThrowIfNull(onOk);
        ThrowIfNull(onErr);
        return _isOk ? onOk(_value) : onErr(_err);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _isOk
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReadOnlySpan<T>.Enumerator GetEnumerator()
    {
        return AsSpan().GetEnumerator();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _isOk
            ? string.Create(CultureInfo.InvariantCulture, $"Ok({_value})")
            : string.Create(CultureInfo.InvariantCulture, $"Err({_err})");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out bool isOk, out T? value, out TErr? err)
    {
        isOk = _isOk;
        value = _value;
        err = _err;
    }

    /// <inheritdoc />
    public bool Equals(Result<T, TErr> other)
    {
        return (_isOk, other._isOk) switch
        {
            (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
            (false, false) => EqualityComparer<TErr>.Default.Equals(_err, other._err),
            _ => false
        };
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Result<T, TErr> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => _isOk ? _value.GetHashCode() : _err.GetHashCode();

    public int CompareTo(Result<T, TErr> other)
    {
        // Ok compares as less than any Err, while two Ok or two Err compare as their contained values would in T or E respectively.
        return (_isOk, other._isOk) switch
        {
            (true, true) => Comparer<T>.Default.Compare(_value, other._value),
            (true, false) => -1,
            (false, true) => 1,
            (false, false) => Comparer<TErr>.Default.Compare(_err, other._err)
        };
    }

    /// <inheritdoc />
    public static bool operator ==(Result<T, TErr> left, Result<T, TErr> right)
        => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(Result<T, TErr> left, Result<T, TErr> right)
        => !left.Equals(right);

    /// <inheritdoc />
    public static bool operator >(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) > 0;

    /// <inheritdoc />
    public static bool operator <(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator >=(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) >= 0;

    /// <inheritdoc />
    public static bool operator <=(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) <= 0;
}

public static class Result
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Ok<T, TErr>(T value)
        where T : notnull where TErr : notnull
    {
        return Result<T, TErr>.Ok(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Err<T, TErr>(TErr error)
        where T : notnull where TErr : notnull
    {
        return Result<T, TErr>.Err(error);
    }
}
