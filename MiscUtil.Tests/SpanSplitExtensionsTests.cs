namespace MiscUtil.Tests;

public class SpanSplitExtensionsTests
{
    [Fact]
    public static void SplitNoMatchSingleResult()
    {
        ReadOnlySpan<char> value = "a b".AsSpan();

        string expected = value.ToString();
        var enumerator = value.Split(',');
        Assert.True(enumerator.MoveNext());
        Assert.Equal(expected, value[enumerator.Current].ToString());
    }

    [Fact]
    public static void DefaultSpanSplitEnumeratorBehavior()
    {
        var charSpanEnumerator = new SpanSplitEnumerator<char>();
        Assert.Equal(new Range(0, 0), charSpanEnumerator.Current);
        Assert.False(charSpanEnumerator.MoveNext());

        // Implicit DoesNotThrow assertion
        charSpanEnumerator.GetEnumerator();
    }

    [Fact]
    public static void ValidateArguments_OverloadWithoutSeparator()
    {
        ReadOnlySpan<char> buffer = default;

        SpanSplitEnumerator<char> enumerator = buffer.Split();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(new Range(0, 0), enumerator.Current);
        Assert.False(enumerator.MoveNext());

        buffer = ReadOnlySpan<char>.Empty;
        enumerator = buffer.Split();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(new Range(0, 0), enumerator.Current);
        Assert.False(enumerator.MoveNext());

        buffer = " ".AsSpan();
        enumerator = buffer.Split();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(new Range(0, 0), enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(new Range(1, 1), enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public static void ValidateArguments_OverloadWithROSSeparator()
    {
        // Default buffer
        ReadOnlySpan<char> buffer = default;

        SpanSplitEnumerator<char> enumerator = buffer.Split(default(char));
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split(' ');
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        // Empty buffer
        buffer = ReadOnlySpan<char>.Empty;

        enumerator = buffer.Split(default(char));
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split(' ');
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        // Single whitespace buffer
        buffer = " ".AsSpan();

        enumerator = buffer.Split(default(char));
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split(' ');
        Assert.Equal(new Range(0, 0), enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(new Range(0, 0), enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(new Range(1, 1), enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public static void ValidateArguments_OverloadWithStringSeparator()
    {
        // Default buffer
        ReadOnlySpan<char> buffer = default;
        string nullString = default;

        SpanSplitEnumerator<char> enumerator = buffer.Split(nullString); // null is treated as empty string
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split("");
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split(" ");
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        // Empty buffer
        buffer = ReadOnlySpan<char>.Empty;

        enumerator = buffer.Split(nullString);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split("");
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split(" ");
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.False(enumerator.MoveNext());

        // Single whitespace buffer
        buffer = " ".AsSpan();

        enumerator = buffer.Split(nullString); // null is treated as empty string
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(1, 1));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split("");
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(1, 1));
        Assert.False(enumerator.MoveNext());

        enumerator = buffer.Split(" ");
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(0, 0));
        Assert.True(enumerator.MoveNext());
        Assert.Equal(enumerator.Current, new Range(1, 1));
        Assert.False(enumerator.MoveNext());
    }

    [Theory]
    [InlineData("", ',', new[] { "" })]
    [InlineData(" ", ' ', new[] { "", "" })]
    [InlineData(",", ',', new[] { "", "" })]
    [InlineData("     ", ' ', new[] { "", "", "", "", "", "" })]
    [InlineData(",,", ',', new[] { "", "", "" })]
    [InlineData("ab", ',', new[] { "ab" })]
    [InlineData("a,b", ',', new[] { "a", "b" })]
    [InlineData("a,", ',', new[] { "a", "" })]
    [InlineData(",b", ',', new[] { "", "b" })]
    [InlineData(",a,b", ',', new[] { "", "a", "b" })]
    [InlineData("a,b,", ',', new[] { "a", "b", "" })]
    [InlineData("a,b,c", ',', new[] { "a", "b", "c" })]
    [InlineData("a,,c", ',', new[] { "a", "", "c" })]
    [InlineData(",a,b,c", ',', new[] { "", "a", "b", "c" })]
    [InlineData("a,b,c,", ',', new[] { "a", "b", "c", "" })]
    [InlineData(",a,b,c,", ',', new[] { "", "a", "b", "c", "" })]
    [InlineData("first,second", ',', new[] { "first", "second" })]
    [InlineData("first,", ',', new[] { "first", "" })]
    [InlineData(",second", ',', new[] { "", "second" })]
    [InlineData(",first,second", ',', new[] { "", "first", "second" })]
    [InlineData("first,second,", ',', new[] { "first", "second", "" })]
    [InlineData("first,second,third", ',', new[] { "first", "second", "third" })]
    [InlineData("first,,third", ',', new[] { "first", "", "third" })]
    [InlineData(",first,second,third", ',', new[] { "", "first", "second", "third" })]
    [InlineData("first,second,third,", ',', new[] { "first", "second", "third", "" })]
    [InlineData(",first,second,third,", ',', new[] { "", "first", "second", "third", "" })]
    [InlineData("Foo Bar Baz", ' ', new[] { "Foo", "Bar", "Baz" })]
    [InlineData("Foo Bar Baz ", ' ', new[] { "Foo", "Bar", "Baz", "" })]
    [InlineData(" Foo Bar Baz ", ' ', new[] { "", "Foo", "Bar", "Baz", "" })]
    [InlineData(" Foo  Bar Baz ", ' ', new[] { "", "Foo", "", "Bar", "Baz", "" })]
    [InlineData("Foo Baz Bar", default(char), new[] { "Foo Baz Bar" })]
    [InlineData("Foo Baz \x0000 Bar", default(char), new[] { "Foo Baz ", " Bar" })]
    [InlineData("Foo Baz \x0000 Bar\x0000", default(char), new[] { "Foo Baz ", " Bar", "" })]
    public static void SpanSplitCharSeparator(string valueParam, char separator, string[] expectedParam)
    {
        char[][] expected = expectedParam.Select(x => x.ToCharArray()).ToArray();
        AssertEqual(expected, valueParam.AsSpan(), valueParam.AsSpan().Split(separator));
    }

    [Theory]
    [InlineData("", new[] { "" })]
    [InlineData(" ", new[] { "", "" })]
    [InlineData("     ", new[] { "", "", "", "", "", "" })]
    [InlineData("  ", new[] { "", "", "" })]
    [InlineData("ab", new[] { "ab" })]
    [InlineData("a b", new[] { "a", "b" })]
    [InlineData("a ", new[] { "a", "" })]
    [InlineData(" b", new[] { "", "b" })]
    [InlineData("Foo Bar Baz", new[] { "Foo", "Bar", "Baz" })]
    [InlineData("Foo Bar Baz ", new[] { "Foo", "Bar", "Baz", "" })]
    [InlineData(" Foo Bar Baz ", new[] { "", "Foo", "Bar", "Baz", "" })]
    [InlineData(" Foo  Bar Baz ", new[] { "", "Foo", "", "Bar", "Baz", "" })]
    public static void SpanSplitDefaultCharSeparator(string valueParam, string[] expectedParam)
    {
        char[][] expected = expectedParam.Select(x => x.ToCharArray()).ToArray();
        AssertEqual(expected, valueParam.AsSpan(), valueParam.AsSpan().Split());
    }

    [Theory]
    [InlineData(" Foo Bar Baz,", ", ", new[] { " Foo Bar Baz," })]
    [InlineData(" Foo Bar Baz, ", ", ", new[] { " Foo Bar Baz", "" })]
    [InlineData(", Foo Bar Baz, ", ", ", new[] { "", "Foo Bar Baz", "" })]
    [InlineData(", Foo, Bar, Baz, ", ", ", new[] { "", "Foo", "Bar", "Baz", "" })]
    [InlineData(", , Foo Bar, Baz", ", ", new[] { "", "", "Foo Bar", "Baz" })]
    [InlineData(", , Foo Bar, Baz, , ", ", ", new[] { "", "", "Foo Bar", "Baz", "", "" })]
    [InlineData(", , , , , ", ", ", new[] { "", "", "", "", "", "" })]
    [InlineData("     ", " ", new[] { "", "", "", "", "", "" })]
    [InlineData("  Foo, Bar  Baz  ", "  ", new[] { "", "Foo, Bar", "Baz", "" })]
    public static void SpanSplitStringSeparator(string valueParam, string separator, string[] expectedParam)
    {
        char[][] expected = expectedParam.Select(x => x.ToCharArray()).ToArray();
        AssertEqual(expected, valueParam.AsSpan(), valueParam.AsSpan().Split(separator));
    }

    [Theory]
    [InlineData("abc_def", '_', "abc", "def")]
    [InlineData("abc_def_ghi", '_', "abc", "def")]
    [InlineData("", '_', "", "")]
    [InlineData("abc_", '_', "abc", "")]
    public static void SpanSplit2_Char(string input, char separator, string expectFirst, string expectSecond)
    {
        var inputSpan = input.AsSpan();
        var (first, second) = inputSpan.Split2(separator);
        Assert.Equal(expectFirst, inputSpan[first].ToString());
        Assert.Equal(expectSecond, inputSpan[second].ToString());
    }

    [Theory]
    [InlineData("abc_def", "_", "abc", "def")]
    [InlineData("abc_def_ghi", "_", "abc", "def")]
    [InlineData("", "_", "", "")]
    [InlineData("abc_", "_", "abc", "")]
    public static void SpanSplit2_String(string input, string separator, string expectFirst, string expectSecond)
    {
        var inputSpan = input.AsSpan();
        var (first, second) = inputSpan.Split2(separator.AsSpan());
        Assert.Equal(expectFirst, inputSpan[first].ToString());
        Assert.Equal(expectSecond, inputSpan[second].ToString());
    }

    [Theory]
    [InlineData("abc_def", '_', "abc", "def", "")]
    [InlineData("abc_def_ghi", '_', "abc", "def", "ghi")]
    [InlineData("abc_def_ghi_jkl", '_', "abc", "def", "ghi")]
    [InlineData("", '_', "", "", "")]
    [InlineData("abc_", '_', "abc", "", "")]
    public static void SpanSplit3_Char(string input, char separator, string expectFirst, string expectSecond, string expectThird)
    {
        var inputSpan = input.AsSpan();
        var (first, second, third) = inputSpan.Split3(separator);
        Assert.Equal(expectFirst, inputSpan[first].ToString());
        Assert.Equal(expectSecond, inputSpan[second].ToString());
        Assert.Equal(expectThird, inputSpan[third].ToString());
    }

    [Theory]
    [InlineData("abc_def", "_", "abc", "def", "")]
    [InlineData("abc_def_ghi", "_", "abc", "def", "ghi")]
    [InlineData("abc_def_ghi_jkl", "_", "abc", "def", "ghi")]
    [InlineData("", "_", "", "", "")]
    [InlineData("abc_", "_", "abc", "", "")]
    public static void SpanSplit3_String(string input, string separator, string expectFirst, string expectSecond, string expectThird)
    {
        var inputSpan = input.AsSpan();
        var (first, second, third) = inputSpan.Split3(separator.AsSpan());
        Assert.Equal(expectFirst, inputSpan[first].ToString());
        Assert.Equal(expectSecond, inputSpan[second].ToString());
        Assert.Equal(expectThird, inputSpan[third].ToString());
    }

    private static void AssertEqual<T>(T[][] items, ReadOnlySpan<T> orig, SpanSplitEnumerator<T> source) where T : IEquatable<T>
    {
        foreach (var item in items)
        {
            Assert.True(source.MoveNext());
            var slice = orig[source.Current];
            Assert.Equal(item, slice.ToArray());
        }
        Assert.False(source.MoveNext());
    }
}
