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
    }
}
