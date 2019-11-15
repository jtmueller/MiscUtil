using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace MiscUtil.Tests
{
    public class SpanExtensionTests
    {
        [Theory]
        [InlineData("0", 0)]
        [InlineData("3912", 3912)]
        [InlineData(" 42 ", 42)]
        [InlineData("-17923568", -17923568)]
        [InlineData("bogus", null)]
        [InlineData("1234abc", null)]
        [InlineData("Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
            "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
            "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null)]
        public void IntParsing(string input, int? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToInt32());
        }

        [Theory]
        [InlineData("0", 0L)]
        [InlineData("3912", 3912L)]
        [InlineData(" 42 ", 42L)]
        [InlineData("-17923568", -17923568L)]
        [InlineData("bogus", null)]
        [InlineData("1234abc", null)]
        [InlineData("Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
            "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
            "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null)]
        public void LongParsing(string input, long? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToLong());
        }

        [Theory]
        [InlineData("0", 0.0)]
        [InlineData("3912.1234", 3912.1234)]
        [InlineData(" 42.8 ", 42.8)]
        [InlineData("-17923568.1", -17923568.1)]
        [InlineData("bogus", null)]
        [InlineData("12345.67abc", null)]
        [InlineData("abc12345.67", null)]
        [InlineData("Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
            "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
            "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null)]
        public void DoubleParsing(string input, double? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToDouble());
        }

        [Theory]
        [InlineData("0", 0.0f)]
        [InlineData("3912.1234", 3912.1234f)]
        [InlineData(" 42.8 ", 42.8f)]
        [InlineData("-17923568.1", -17923568.1f)]
        [InlineData("bogus", null)]
        [InlineData("12345.67abc", null)]
        [InlineData("abc12345.67", null)]
        [InlineData("Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
            "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
            "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null)]
        public void FloatParsing(string input, float? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToFloat());
        }

        public static IEnumerable<object[]> DecimalData()
        {
            yield return new object[] { "0", 0.0M };
            yield return new object[] { "3912.1234", 3912.1234M };
            yield return new object[] { " 42.8 ", 42.8M };
            yield return new object[] { " -17.1", -17.1M };
            yield return new object[] { "bogus", null };
            yield return new object[] { "12345.67abc", null };
            yield return new object[] { "Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
                "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
                "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null };
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
        [InlineData("TrueButNotReally", null)]
        [InlineData("Really long string that is far beyond the maximum size for a stack-allocated byte array. " + 
            "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
            "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null)]
        public void BoolParsing(string input, bool? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToBoolean());
        }

        public static IEnumerable<object[]> GuidData()
        {
            foreach (var f in new[] { "N", "D", "B", "P" })
            {
                var g = Guid.NewGuid();
                yield return new object[] { g.ToString(f), g };
            }
            yield return new object[] { "abcdefg", null };
            yield return new object[] { " 286f4642-17fe-4930-86c5-d171c8ca74d2 ", new Guid("286f4642-17fe-4930-86c5-d171c8ca74d2") };
            yield return new object[] { "Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
                "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
                "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", null };
        }

        [Theory]
        [MemberData(nameof(GuidData))]
        public void GuidParsing(string input, Guid? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToGuid());
        }

        public static IEnumerable<object[]> DateTimeData()
        {
            // Utf8Parser only supports these formats:
#if NET48
            foreach (var f in new[] { "G", "O", "R" })
#else
            foreach (var f in new[] { "d", "D", "f", "F", "g", "G", "M", "O", "R", "s", "t", "T", "u", "U", "y" })
#endif
            {
                var ds = DateTime.Now.ToString(f, CultureInfo.InvariantCulture); // the only culture supported by Utf8Parser
                yield return new object[] { ds, f[0], DateTime.ParseExact(ds, f, CultureInfo.InvariantCulture) };
            }
            yield return new object[] { "abcdefg", '\0', null };
            yield return new object[] { "Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
                "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
                "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", '\0', null };
        }

        [Theory]
        [MemberData(nameof(DateTimeData))]
        public void DateTimeParsing(string input, char format, DateTime? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToDateTime(format));
        }

        public static IEnumerable<object[]> DateTimeOffsetData()
        {
            // Utf8Parser only supports these formats:
#if NET48
            foreach (var f in new[] { "G", "R" })
#else
            foreach (var f in new[] { "d", "D", "f", "F", "g", "G", "M", "R", "s", "t", "T", "u", "y" })
#endif
            {
                var ds = DateTimeOffset.Now.ToString(f, CultureInfo.InvariantCulture); // the only culture supported by Utf8Parser
                yield return new object[] { ds, f[0], DateTimeOffset.ParseExact(ds, f, CultureInfo.InvariantCulture) };
            }
            yield return new object[] { "abcdefg", '\0', null };
            yield return new object[] { "Really long string that is far beyond the maximum size for a stack-allocated byte array. " +
                "I really can't tell you how silly it is to attempt to parse a string like this into a value, " +
                "but we can't crash if some idiot tries. And you know they will try. Can't stop them, idiots.", '\0', null };
        }

        [Theory]
        [MemberData(nameof(DateTimeOffsetData))]
        public void DateTimeOffsetParsing(string input, char format, DateTimeOffset? expected)
        {
            Assert.Equal(expected, input.AsSpan().ToDateTimeOffset(format));
        }

        [Theory]
        [InlineData("abc def", "abc def")]
        [InlineData(" abcdef ", "abcdef   ")]
        [InlineData("\tabcdef\n", " abcdef\t\t\r\n")]
        [InlineData("abc ", " def")]
        public void TrimEquals(string input1, string input2)
        {
            Assert.Equal(input1.Trim().Equals(input2.Trim(), StringComparison.Ordinal), input1.TrimEquals(input2, StringComparison.Ordinal));
        }
    }
}
