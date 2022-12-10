﻿using Xunit;
using static MiscUtil.Result;

namespace MiscUtil.Tests;

public sealed class ResultTests
{
    [Fact]
    public void CanPerformBasicOperationsStructClass()
    {
        var okInt = Result<int, string>.Ok(42);
        var errStr = Result<int, string>.Err("Whoops!");

        var okInt2 = Ok<int, string>(42);
        var errStr2 = Err<int, string>("Whoops!");

        Assert.True(okInt.IsOk(out var o1) && o1 == 42);
        Assert.True(errStr.IsErr(out var e1) && e1 == "Whoops!");
        Assert.False(okInt.IsErr(out _));
        Assert.False(errStr.IsOk(out _));

        Assert.True(okInt == okInt2);
        Assert.Equal(okInt, okInt2);
        Assert.True(okInt.Equals(okInt2));
    }

    [Fact]
    public void CanPerformBasicOperationsClassStruct()
    {
        var okStr = Result<string, int>.Ok("Foo");
        var errInt = Result<string, int>.Err(-1);

        var okInt2 = Ok<string, int>("Foo");
        var errStr2 = Err<string, int>(-1);

        Assert.True(okStr.IsOk(out var o1) && o1 == "Foo");
        Assert.True(errInt.IsErr(out var e1) && e1 == -1);
        Assert.False(okStr.IsErr(out _));
        Assert.False(errInt.IsOk(out _));

        Assert.True(okStr == okInt2);
        Assert.Equal(okStr, okInt2);
        Assert.True(okStr.Equals(okInt2));
    }
}

