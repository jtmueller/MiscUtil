﻿using System;
using System.Globalization;
using System.Text;
using Xunit;

namespace MiscUtil.Tests
{
    public class StringBuilderExtensionsTests
    {
        [Theory]
        [InlineData("abcdefg,", new char[] { ',' })]
        [InlineData("abcdefg,,,", new char[] { ',' })]
        [InlineData("abcdefg|", new char[] { ',', '|' })]
        [InlineData("abcdefg,|", new char[] { ',', '|' })]
        [InlineData("abcdefg ", null)]
        [InlineData("abcdefg     ", new char[0])]
        [InlineData("abcdefg \t\n ", null)]
        public void TrimEnd(string str, char[] chars)
        {
            Assert.Equal(str.TrimEnd(chars), new StringBuilder(str).TrimEnd(chars).ToString());
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
}
