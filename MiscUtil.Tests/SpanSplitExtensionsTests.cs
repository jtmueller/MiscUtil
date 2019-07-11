using System;
using System.Collections.Generic;
using Xunit;

namespace MiscUtil.Tests
{
    public class SpanSplitExtensionsTests
    {
        [Theory]
        [InlineData("", 'a')]
        [InlineData("a", 'a')]
        [InlineData("aaaa", 'a')]
        [InlineData("abababab", 'a')]
        [InlineData("babababa", 'a')]
        [InlineData("aaaaab", 'a')]
        [InlineData("baaaaa", 'a')]
        [InlineData("zzzzzazzzzazzzzazzz", 'a')]
        public void SplitOneChar(string str, char separator)
        {
            var parts = new List<string>();

            foreach (var part in str.AsSpan().Split(separator))
                parts.Add(part.ToString());

            Assert.Equal(str.Split(separator), parts);
        }

        [Theory]
        [InlineData("", 'a')]
        [InlineData("a", 'a')]
        [InlineData("aaaa", 'a')]
        [InlineData("abababab", 'a')]
        [InlineData("babababa", 'a')]
        [InlineData("aaaaab", 'a')]
        [InlineData("baaaaa", 'a')]
        [InlineData("zzzzzazzzzazzzzazzz", 'z')]
        public void SplitOneCharRemoveEmpty(string str, char separator)
        {
            var parts = new List<string>();

            foreach (var part in str.AsSpan().Split(separator, SpanSplitOptions.RemoveEmptyEntries))
                parts.Add(part.ToString());

            Assert.Equal(str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries), parts);
        }

        [Theory]
        [InlineData("aaababbaaabbabbaba", "bb")]
        [InlineData("aaaa\r\nbbb\r\ncccc\ncc\r\ndddd", "\r\n")]
        public void SplitAllChars(string str, string separator)
        {
            var parts = new List<string>();

            foreach (var part in str.AsSpan().SplitAll(separator.AsSpan()))
                parts.Add(part.ToString());

            Assert.Equal(str.Split(new[] { separator }, StringSplitOptions.None), parts);
        }
    }
}
