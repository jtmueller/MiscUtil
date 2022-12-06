﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.ArgumentNullException;

namespace MiscUtil;

public readonly struct Result<T, TErr> : IEquatable<Result<T, TErr>>
    where T : notnull where TErr : notnull
{
    public static Result<T, TErr> Ok(T value) => new(value);
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

    private readonly T _value;
    private readonly bool _isOk;
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
    public ReadOnlySpan<T> AsSpan()
    {
        return _isOk
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T>.Enumerator GetEnumerator()
    {
        return AsSpan().GetEnumerator();
    }

    /// <inheritdoc />
    public bool Equals(Result<T, TErr> other)
    {
        if (_isOk != other._isOk)
            return false;

        if (_isOk)
        {
            if (_value is IEquatable<T> eq)
                return eq.Equals(other._value);

            return _value.Equals(other._value);
        }
        else
        {
            if (_err is IEquatable<TErr> eq)
                return eq.Equals(other._err);

            return _err.Equals(other._err);
        }
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Result<T, TErr> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _isOk ? _value.GetHashCode() : _err.GetHashCode();
    }
}

public static class Result
{
    public static Result<T, TErr> Create<T, TErr>(T? value, TErr? error)
        where T : notnull where TErr : notnull
    {
        return (value, error) switch
        {
            (T, _) => Result<T, TErr>.Ok(value),
            (_, TErr) => Result<T, TErr>.Err(error),
            _ => throw new InvalidOperationException("Either the value or the error must be non-null.")
        };
    }

    public static Result<T, TErr> Create<T, TErr>(T? value, Func<TErr> errorFactory)
        where T : notnull where TErr : notnull
    {
        ThrowIfNull(errorFactory);

        return value is null 
            ? Result<T, TErr>.Err(errorFactory()) 
            : Result<T, TErr>.Ok(value);
    }

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

// TODO: useful methods from https://doc.rust-lang.org/std/result/
// expect, err, ok, transpose, unwrap_or, map_or, and, or
// Also a .Ok() extension method on any type?

public static class ResultExtensions
{
    public static U Match<T, TErr, U>(this Result<T, TErr> result,
                                      Func<T, U> onOk,
                                      Func<TErr, U> onErr)
        where T : notnull where TErr : notnull where U : notnull
    {
        return result.IsOk(out var value)
            ? onOk(value)
            : result.IsErr(out var error)
                ? onErr(error)
                : throw new InvalidOperationException("Result not in valid state.");
    }

    public static Result<U, UErr> Bind<T, TErr, U, UErr>(this Result<T, TErr> result,
                                                         Func<T, Result<U, UErr>> okBinder,
                                                         Func<TErr, Result<U, UErr>> errBinder)
        where T : notnull where TErr : notnull where U : notnull where UErr : notnull
    {
        return result.IsOk(out var value)
            ? okBinder(value)
            : result.IsErr(out var error)
                ? errBinder(error)
                : throw new InvalidOperationException("Result not in valid state.");
    }

    public static Result<U, TErr> Map<T, TErr, U>(this Result<T, TErr> result, Func<T, U> mapper)
        where T : notnull where TErr : notnull where U : notnull
    {
        return result.IsOk(out var value)
            ? Result<U, TErr>.Ok(mapper(value))
            : result.IsErr(out var error)
                ? Result<U, TErr>.Err(error)
                : throw new InvalidOperationException("Result not in valid state.");
    }

    public static Result<T, UErr> MapErr<T, TErr, UErr>(this Result<T, TErr> result, Func<TErr, UErr> errMapper)
        where T : notnull where TErr : notnull where UErr : notnull
    {
        return result.IsErr(out var error)
            ? Result<T, UErr>.Err(errMapper(error))
            : result.IsOk(out var value)
                ? Result<T, UErr>.Ok(value)
                : throw new InvalidOperationException("Result not in valid state.");
    }
}
