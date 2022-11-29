namespace MiscUtil.Tests;

public class DisposeTests
{
    [Fact]
    public void CanDispose()
    {
        bool disposed = false;
        using (var sut = Disposable.Create(() => { disposed = true; }))
        {
            Assert.False(disposed);
        }
        Assert.True(disposed);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public async Task CanDisposeAsync()
    {
        bool disposed = false;
        await using (var sut = AsyncDisposable.Create(async () => { await Task.Yield(); disposed = true; }))
        {
            Assert.False(disposed);
        }
        Assert.True(disposed);
    }
#endif
}
