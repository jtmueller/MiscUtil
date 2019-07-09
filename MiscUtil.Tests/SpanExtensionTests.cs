using System;
using System.Collections.Generic;
using Xunit;

namespace MiscUtil.Tests
{
    public class SpanExtensionTests
    {
        [Theory]
        [InlineData("0", 0)]
        [InlineData("3912", 3912)]
        [InlineData(" 42 ", 42)]
        [InlineData(" -17", -17)]
        [InlineData("bogus", null)]
        public void IntParsing(string input, int? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToInt32());
        }

        [Theory]
        [InlineData("0", 0.0)]
        [InlineData("3912.1234", 3912.1234)]
        [InlineData(" 42.8 ", 42.8)]
        [InlineData(" -17.1", -17.1)]
        [InlineData("bogus", null)]
        public void DoubleParsing(string input, double? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToDouble());
        }

        public static IEnumerable<object[]> DecimalData()
        {
            yield return new object[] { "0", 0.0M };
            yield return new object[] { "3912.1234", 3912.1234M };
            yield return new object[] { " 42.8 ", 42.8M };
            yield return new object[] { " -17.1", -17.1M };
            yield return new object[] { "bogus", null };
        }

        [Theory]
        [MemberData(nameof(DecimalData))]
        public void DecimalParsing(string input, decimal? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToDecimal());
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData(" True ", true)]
        [InlineData(" False ", false)]
        [InlineData("bogus", null)]
        public void BoolParsing(string input, bool? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToBoolean());
        }

        [Theory]
        [InlineData("abcdef", "abcdef")]
        [InlineData(" abcdef ", "abcdef   ")]
        [InlineData("\tabcdef\n", " abcdef\t\t\r\n")]
        public void TrimEquals(string input1, string input2)
        {
            Assert.Equal(input1.Trim().Equals(input2.Trim(), StringComparison.Ordinal), input1.TrimEquals(input2, StringComparison.Ordinal));
        }
    }
}
