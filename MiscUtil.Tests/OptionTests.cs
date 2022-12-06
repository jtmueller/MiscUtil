#if NET6_0_OR_GREATER

namespace MiscUtil.Tests;

using Xunit;

public class OptionTests
{
    [Fact]
    public void CanPerformBasicOperations()
    {
        var none = Option<int>.None;
        var someStruct = Option.Some(42);
        var someClass = Option<string>.Some(new string("test"));

        Assert.True(none.IsNone);
        Assert.False(none.IsSome(out _));

        Assert.True(someStruct.IsSome(out var structVal));
        Assert.Equal(42, structVal);
        Assert.False(someStruct.IsNone);

        Assert.True(someClass.IsSome(out var classVal));
        Assert.Equal("test", classVal);
        Assert.False(someClass.IsNone);
    }

    [Fact]
    public void CanMatchOnSome()
    {
        var opt = Option.Some(42);
        var result = opt.Match(onSome: x => x * 2, onNone: () => -1);
        Assert.Equal(84, result);
    }

    [Fact]
    public void CanMatchOnNone()
    {
        var opt = Option<int>.None;
        var result = opt.Match(onSome: x => x * 2, onNone: () => -1);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void CanBind()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someResult = someInt.Bind(x => Option.Some(x.ToString()));
        var noneResult = noneInt.Bind(x => Option.Some(x.ToString()));
        var someToNoneResult = someInt.Bind(_ => Option<DateTime>.None);

        Assert.True(someResult.IsSome(out var str));
        Assert.Equal("42", str);
        Assert.True(noneResult.IsNone);
        Assert.False(someToNoneResult.IsSome(out _));
    }

    [Fact]
    public void CanMap()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someResult = someInt.Map(x => x.ToString());
        var noneResult = noneInt.Map(x => x.ToString());

        Assert.True(someResult.IsSome(out var str));
        Assert.Equal("42", str);
        Assert.True(noneResult.IsNone);
    }

    [Fact]
    public void CanGetSpan()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        var someSpan = someInt.AsSpan();
        var noneSpan = noneInt.AsSpan();

        Assert.False(someSpan.IsEmpty);
        Assert.True(noneSpan.IsEmpty);

        Assert.Equal(1, someSpan.Length);
        Assert.Equal(0, noneSpan.Length);

        Assert.Equal(42, someSpan[0]);
    }

    [Fact]
    public void CanEnumerate()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        int value = 0;
        foreach (var x in noneInt)
        {
            value += x;
        }

        Assert.Equal(0, value);

        foreach (var x in someInt)
        {
            value += x;
        }

        Assert.Equal(42, value);
    }

#if NET7_0_OR_GREATER

    [Fact]
    public void CanParseStrings()
    {
        var integer = Option.Parse<int>("12345");
        var date = Option.Parse<DateTime>("2023-06-17");
        var timespan = Option.Parse<TimeSpan>("05:11:04");
        var fraction = Option.Parse<double>("3.14");
        var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965");

        Assert.True(integer.IsSome(out var i) && i == 12345);
        Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
        Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
        Assert.True(fraction.IsSome(out var x) && x == 3.14);
        Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
    }

    [Fact]
    public void CanParseSpans()
    {
        var integer = Option.Parse<int>("12345".AsSpan());
        var date = Option.Parse<DateTime>("2023-06-17".AsSpan());
        var timespan = Option.Parse<TimeSpan>("05:11:04".AsSpan());
        var fraction = Option.Parse<double>("3.14".AsSpan());
        var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965".AsSpan());

        Assert.True(integer.IsSome(out var i) && i == 12345);
        Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
        Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
        Assert.True(fraction.IsSome(out var x) && x == 3.14);
        Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
    }

#endif
}

#endif