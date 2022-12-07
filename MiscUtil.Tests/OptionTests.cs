namespace MiscUtil.Tests;

using System.Globalization;
using Xunit;

public class OptionTests
{
    [Fact]
    public void CanPerformBasicOperations()
    {
        var none = Option<int>.None;
        var someStruct = Option.Some(42);
        var someNullableStruct = Option.Create((int?)42);
        var someClass = Option<string>.Some(new string("test"));
        var nullOptClass = Option.Create((string?)null);
        var nullOptStruct = Option.Create((int?)null);

        Assert.True(none.IsNone);
        Assert.False(none.IsSome(out _));

        Assert.True(someStruct.IsSome(out var structVal));
        Assert.Equal(42, structVal);
        Assert.False(someStruct.IsNone);

        Assert.True(someNullableStruct.IsSome(out var ns));
        Assert.Equal(42, ns);
        Assert.False(someNullableStruct.IsNone);

        Assert.True(someClass.IsSome(out var classVal));
        Assert.Equal("test", classVal);
        Assert.False(someClass.IsNone);

        Assert.True(nullOptClass.IsNone);
        Assert.True(nullOptStruct.IsNone);
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
        var opt = Option.None<int>();
        var result = opt.Match(onSome: x => x * 2, onNone: () => -1);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void CanBind()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someResult = someInt.Bind(x => Option.Create(x.ToString()));
        var noneResult = noneInt.Bind(x => Option.Create(x.ToString()));
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
    public void CanMapOr()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someResult = someInt.MapOr(x => x.ToString(), "empty");
        var noneResult = noneInt.MapOr(x => x.ToString(), "empty");

        Assert.Equal("42", someResult);
        Assert.Equal("empty", noneResult);
    }

    [Fact]
    public void CanMapOrElse()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someResult = someInt.MapOrElse(x => x.ToString(), () => "empty");
        var noneResult = noneInt.MapOrElse(x => x.ToString(), () => "empty");

        Assert.Equal("42", someResult);
        Assert.Equal("empty", noneResult);
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

    [Fact]
    public void CanExpect()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        Assert.Equal(42, someInt.Expect("Needs a value"));
        var ex = Assert.Throws<InvalidOperationException>(() => _ = noneInt.Expect("Needs a value"));
        Assert.Equal("Needs a value", ex.Message);
    }

    [Fact]
    public void CanUnwrap()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        Assert.Equal(42, someInt.Unwrap());
        _ = Assert.Throws<InvalidOperationException>(() => _ = noneInt.Unwrap());
    }

    [Fact]
    public void CanUnwrapOrDefault()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        Assert.Equal(42, someInt.UnwrapOr(-1));
        Assert.Equal(0, noneInt.UnwrapOr(default));
    }

    [Fact]
    public void CanUnwrapOrElse()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        Assert.Equal(42, someInt.UnwrapOrElse(() => -1));
        Assert.Equal(-1, noneInt.UnwrapOrElse(() => -1));
    }

    [Fact]
    public void CanEquate()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someSameInt = Option.Some(42);
        var someOtherInt = Option.Some(4);

        Assert.Equal(someInt, someSameInt);
        Assert.NotEqual(someInt, noneInt);
        Assert.NotEqual(someInt, someOtherInt);

        Assert.True(someInt == someSameInt);
        Assert.True(someInt != noneInt);
        Assert.False(someInt == someOtherInt);

        Assert.True(((object)someInt).Equals(someSameInt));
        Assert.False(((object)someInt).Equals(noneInt));
        Assert.False(((object)someInt).Equals(someOtherInt));

        Assert.Equal(someInt.GetHashCode(), someSameInt.GetHashCode());
        Assert.NotEqual(someInt.GetHashCode(), noneInt.GetHashCode());
        Assert.NotEqual(someInt.GetHashCode(), someOtherInt.GetHashCode());
    }

    [Fact]
    public void CanTransformToResultVal()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        var someResult = someInt.OkOr("No value found!");
        var noneResult = noneInt.OkOr("No value found!");

        Assert.True(someResult.IsOk(out var value) && value == 42);
        Assert.True(noneResult.IsErr(out var err) && err == "No value found!");
    }

    [Fact]
    public void CanTransformToResultFunc()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;

        var someResult = someInt.OkOrElse(() => "No value found!");
        var noneResult = noneInt.OkOrElse(() => "No value found!");

        Assert.True(someResult.IsOk(out var value) && value == 42);
        Assert.True(noneResult.IsErr(out var err) && err == "No value found!");
    }

    [Fact]
    public void CanTranspose()
    {
        var someOkTest = Option<Result<int, string>>.Some(Result<int, string>.Ok(42));
        var someErrTest = Option<Result<int, string>>.Some(Result<int, string>.Err("Bad things happened"));
        var noneTest = Option<Result<int, string>>.None;

        var someOkExpected = Result<Option<int>, string>.Ok(Option.Some(42));
        var someErrExpected = Result<Option<int>, string>.Err("Bad things happened");
        var noneExpected = Result<Option<int>, string>.Ok(Option<int>.None);

        Assert.Equal(someOkExpected, someOkTest.Transpose());
        Assert.Equal(someErrExpected, someErrTest.Transpose());
        Assert.Equal(noneExpected, noneTest.Transpose());
    }

    [Fact]
    public void CanGetString()
    {
        var someInt = Option.Some(4200);
        var noneInt = Option<int>.None;

        Assert.Equal("Some(4200)", someInt.ToString());
        Assert.Equal("None", noneInt.ToString());
        Assert.Equal("Some(4,200.00)", someInt.ToString("n2", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void CanFormatToSpan()
    {
        var someInt = Option.Some(4200);
        var noneInt = Option<int>.None;

        Span<char> buffer = stackalloc char[255];

        Assert.True(someInt.TryFormat(buffer, out int written, "", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Some(4200)"));

        Assert.True(noneInt.TryFormat(buffer, out written, "", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("None"));

        Assert.True(someInt.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Some(4,200.00)"));
    }

    [Fact]
    public void CanFlatten()
    {
        var someInt = Option.Some(Option.Some(42));
        var noneIntOuter = Option<Option<int>>.None;
        var noneIntInner = Option.Some(Option<int>.None);
        var someThreeLevels = Option.Some(Option.Some(Option.Some(42)));

        Assert.Equal(Option.Some(42), someInt.Flatten());
        Assert.Equal(Option<int>.None, noneIntOuter.Flatten());
        Assert.Equal(Option<int>.None, noneIntInner.Flatten());
        Assert.Equal(someInt, someThreeLevels.Flatten());
    }

    [Fact]
    public void CanFilter()
    {
        var someInt = Option.Some(42);
        var noneInt = Option<int>.None;
        var someOtherInt = Option.Some(43);

        Assert.Equal(someInt, someInt.Filter(x => x % 2 == 0));
        Assert.Equal(noneInt, noneInt.Filter(x => x % 2 == 0));
        Assert.Equal(noneInt, someOtherInt.Filter(x => x % 2 == 0));
    }

    [Fact]
    public void CanZip()
    {
        var x = Option.Some(42);
        var y = Option.Some(17);

        var result = x.Zip(y);
        var result2 = x.Zip(Option<int>.None);
        var result3 = Option<int>.None.Zip(x);

        Assert.True(result.IsSome(out var value) && value == (42, 17));
        Assert.True(result2.IsNone);
        Assert.True(result3.IsNone);
    }

    [Fact]
    public void CanZipWith()
    {
        var x = Option.Some("key");
        var y = Option.Some(17);

        var result = x.ZipWith(y, KeyValuePair.Create);
        var result2 = x.ZipWith(Option<int>.None, KeyValuePair.Create);
        var result3 = Option<int>.None.ZipWith(x, KeyValuePair.Create);

        Assert.True(result.IsSome(out var value));
        Assert.Equal("key", value.Key);
        Assert.Equal(17, value.Value);
        Assert.True(result2.IsNone);
        Assert.True(result3.IsNone);
    }

    [Fact]
    public void CanAnd()
    {
        var someStr = Option.Some("42");
        var noneStr = Option<string>.None;

        Assert.Equal(Option.Some(17), someStr.And(Option.Some(17)));
        Assert.Equal(Option<int>.None, noneStr.And(Option.Some(17)));
        Assert.Equal(Option<int>.None, someStr.And(Option<int>.None));
    }

    [Fact]
    public void CanAndThen()
    {
        var someIntStr = Option.Some("42");
        var someOtherStr = Option.Some("foo");
        var noneStr = Option<string>.None;

        Assert.Equal(Option.Some(42), someIntStr.AndThen(ParseInt));
        Assert.Equal(Option<int>.None, noneStr.AndThen(ParseInt));
        Assert.Equal(Option<int>.None, someOtherStr.AndThen(ParseInt));

        static Option<int> ParseInt(string s)
            => int.TryParse(s, out int parsed) ? Option.Some(parsed) : Option<int>.None;
    }

    [Fact]
    public void CanOr()
    {
        var someStr = Option.Some("42");
        var noneStr = Option<string>.None;

        Assert.Equal(someStr, someStr.Or(Option.Some("other")));
        Assert.Equal(Option.Some("other"), noneStr.Or(Option.Some("other")));
    }

    [Fact]
    public void CanOrElse()
    {
        var someStr = Option.Some("42");
        var noneStr = Option<string>.None;

        Assert.Equal(someStr, someStr.OrElse(() => Option.Some("other")));
        Assert.Equal(Option.Some("other"), noneStr.OrElse(() => Option.Some("other")));
    }

    [Fact]
    public void CanXor()
    {
        var sx = Option.Some(42);
        var sy = Option.Some(17);
        var nn = Option<int>.None;

        Assert.Equal(nn, nn.Xor(nn));
        Assert.Equal(sy, nn.Xor(sy));
        Assert.Equal(sx, sx.Xor(nn));
        Assert.Equal(nn, sx.Xor(sy));
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
        var nothing = Option.Parse<int>("foo");

        Assert.True(integer.IsSome(out var i) && i == 12345);
        Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
        Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
        Assert.True(fraction.IsSome(out var x) && x == 3.14);
        Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
        Assert.True(nothing.IsNone);
    }

    [Fact]
    public void CanParseSpans()
    {
        var integer = Option.Parse<int>("12345".AsSpan());
        var date = Option.Parse<DateTime>("2023-06-17".AsSpan());
        var timespan = Option.Parse<TimeSpan>("05:11:04".AsSpan());
        var fraction = Option.Parse<double>("3.14".AsSpan());
        var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965".AsSpan());
        var nothing = Option.Parse<int>("foo".AsSpan());

        Assert.True(integer.IsSome(out var i) && i == 12345);
        Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
        Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
        Assert.True(fraction.IsSome(out var x) && x == 3.14);
        Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
        Assert.True(nothing.IsNone);
    }

#endif
}
