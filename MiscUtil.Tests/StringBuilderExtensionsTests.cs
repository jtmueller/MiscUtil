using System.Globalization;
using System.Text;
using Xunit;

namespace MiscUtil.Tests;

public class StringBuilderExtensionsTests
{
    [Theory]
    [InlineData("abcdefg,", new[] { ',' })]
    [InlineData("abcdefg,,,", new[] { ',' })]
    [InlineData("abcdefg|", new[] { ',', '|' })]
    [InlineData("abcdefg,|", new[] { ',', '|' })]
    [InlineData("abcdefg ", null)]
    [InlineData("abcdefg     ", new char[0])]
    [InlineData("abcdefg \t\n ", null)]
    public void TrimEnd(string str, char[] chars)
    {
        Assert.Equal(str.TrimEnd(chars), new StringBuilder(str).TrimEnd(chars).ToString());
    }

    [Theory]
    [InlineData(",abcdefg", new[] { ',' })]
    [InlineData(",,,abcdefg", new[] { ',' })]
    [InlineData("|abcdefg", new[] { ',', '|' })]
    [InlineData(",|abcdefg", new[] { ',', '|' })]
    [InlineData(" abcdefg", null)]
    [InlineData("     abcdefg", new char[0])]
    [InlineData(" \t\n abcdefg", null)]
    public void TrimStart(string str, char[] chars)
    {
        Assert.Equal(str.TrimStart(chars), new StringBuilder(str).TrimStart(chars).ToString());
    }

    [Theory]
    [InlineData(",abcdefg,", new[] { ',' })]
    [InlineData(",,,abcdefg,,,", new[] { ',' })]
    [InlineData("|abcdefg|", new[] { ',', '|' })]
    [InlineData(",|abcdefg|,", new[] { ',', '|' })]
    [InlineData(" abcdefg ", null)]
    [InlineData("     abcdefg     ", new char[0])]
    [InlineData(" \t\n abcdefg \t\n ", null)]
    public void Trim(string str, char[] chars)
    {
        Assert.Equal(str.Trim(chars), new StringBuilder(str).Trim(chars).ToString());
    }

    [Fact]
    public void AppendSpan()
    {
        var alphabet = "abcdefghijklmnopqrstuvwxyz";
        var sb = new StringBuilder();
        sb.Append(alphabet.AsSpan());
        Assert.Equal(alphabet, sb.ToString());

        var partial = alphabet.AsSpan(10);
        sb.Append(partial);
        Assert.Equal(alphabet + alphabet.Substring(10), sb.ToString());
    }

    [Fact]
    public void CopyTo()
    {
        var sb = new StringBuilder("abcdefghijklmnopqrstuvwxyz");
        Span<char> dest = stackalloc char[sb.Length];
        sb.CopyTo(0, dest, sb.Length);

        Assert.Equal(sb.ToString(), dest.ToString());
    }

    [Theory]
    [InlineData("abcdefg")]
    [InlineData(" abcDEFghi ")]
    public void ToUpperInvariant(string str)
    {
        Assert.Equal(str.ToUpperInvariant(), new StringBuilder(str).ToUpperInvariant().ToString());
    }

    [Theory]
    [InlineData("abcdefg")]
    [InlineData(" abcDEFghi ")]
    public void ToLowerInvariant(string str)
    {
        Assert.Equal(str.ToLowerInvariant(), new StringBuilder(str).ToLowerInvariant().ToString());
    }

    [Theory]
    [InlineData("abcdefg")]
    [InlineData(" abcDEFghi ")]
    public void ToUpper(string str)
    {
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        Assert.Equal(str.ToUpper(culture), new StringBuilder(str).ToUpper(culture).ToString());
    }

    [Theory]
    [InlineData("abcdefg")]
    [InlineData(" abcDEFghi ")]
    public void ToLower(string str)
    {
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        Assert.Equal(str.ToLower(culture), new StringBuilder(str).ToLower(culture).ToString());
    }
}
