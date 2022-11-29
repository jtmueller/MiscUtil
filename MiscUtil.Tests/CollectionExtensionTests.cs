#if NET6_0_OR_GREATER

namespace MiscUtil.Tests;

using System.Collections.ObjectModel;
using System.Globalization;
using MiscUtil;
using Xunit;

public class CollectionExtensionTests
{
    [Fact]
    public void CanGetOrAddWithValue()
    {
        var dict = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 }
        };

        Assert.Equal(1, dict.GetOrAdd("one", 5));
        Assert.Equal(5, dict.GetOrAdd("five", 5));
        Assert.Equal(3, dict.Count);
    }

    [Fact]
    public void CanGetOrAddWithFactory()
    {
        var dict = new Dictionary<int, string>
        {
            { 1, "1" },
            { 2, "2" }
        };

        Assert.Equal("1", dict.GetOrAdd(1, _ => "one"));
        Assert.Equal("5", dict.GetOrAdd(5, x => x.ToString(CultureInfo.InvariantCulture)));
        Assert.Equal(3, dict.Count);
    }

    [Fact]
    public void ExceptionInFactoryPreventsAdd()
    {
        var dict = new Dictionary<int, string>
        {
            { 1, "1" },
            { 2, "2" }
        };

        Assert.Throws<InvalidOperationException>(() => dict.GetOrAdd(5, x => throw new InvalidOperationException("whoops!")));
        Assert.Equal(2, dict.Count);
        Assert.False(dict.ContainsKey(5));
    }

    [Fact]
    public void CanAddOrUpdateWithValue()
    {
        var dict = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 1 }
        };

        Assert.Equal(1, dict.AddOrUpdate("three", 1, Increment));
        Assert.Equal(2, dict.AddOrUpdate("three", 1, Increment));
        Assert.Equal(3, dict.AddOrUpdate("three", 1, Increment));
        Assert.Equal(3, dict.Count);

        static int Increment(string key, int x) => x + 1;
    }

    [Fact]
    public void CanAddOrUpdateWithFactory()
    {
        var dict = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 1 }
        };

        Assert.Equal(5, dict.AddOrUpdate("three", key => key.Length, Increment));
        Assert.Equal(6, dict.AddOrUpdate("three", key => key.Length, Increment));
        Assert.Equal(7, dict.AddOrUpdate("three", key => key.Length, Increment));
        Assert.Equal(3, dict.Count);

        static int Increment(string key, int x) => x + 1;
    }

    [Fact]
    public void CanAddRangeWithList()
    {
        IList<int> list = new List<int>();

        list.AddRange(new[] { 1, 2, 3, 4, 5 });

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, list);
    }

    [Fact]
    public void CanAddRangeWithCollection()
    {
        IList<int> list = new Collection<int>();

        list.AddRange(new[] { 1, 2, 3, 4, 5 });

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, list);
    }

    [Fact]
    public void CanRemoveAllWithList()
    {
        IList<int> list = new List<int>();
        list.AddRange(new[] { 1, 2, 3, 4, 5 });
        Assert.Equal(5, list.Count);

        list.RemoveAll(x => x % 2 == 0);

        Assert.Equal(new[] { 1, 3, 5 }, list);
    }

    [Fact]
    public void CanRemoveAllWithCollection()
    {
        IList<int> list = new Collection<int>();
        list.AddRange(new[] { 1, 2, 3, 4, 5 });
        Assert.Equal(5, list.Count);

        list.RemoveAll(x => x % 2 == 0);

        Assert.Equal(new[] { 1, 3, 5 }, list);
    }
}

#endif
