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
        public void Test(string str, char separator)
        {
            var parts = new List<string>();

            foreach (var part in str.AsSpan().Split(separator))
                parts.Add(part.ToString());

            Assert.Equal(str.Split(separator), parts);
        }
    }
}
