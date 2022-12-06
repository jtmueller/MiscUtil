using BenchmarkDotNet.Attributes;

namespace MiscUtil.Benchmarks;

[Config(typeof(BenchmarkConfig))]
public class GuidParsing
{
    [Benchmark(Baseline = true)]
    public void GuidStringParse()
    {
        for (int i = 0; i < _toParse.Length; i++)
        {
            _ = Guid.TryParse(_toParse[i], out _);
        }
    }

    [Benchmark]
    public void GuidSpanParse()
    {
        for (int i = 0; i < _toParse.Length; i++)
        {
            _ = _toParse[i].AsSpan().ToGuid();
        }
    }

#if NET7_0_OR_GREATER

    [Benchmark]
    public void GuidOptionParse()
    {
        for (int i = 0; i < _toParse.Length; i++)
        {
            _ = Option.Parse<Guid>(_toParse[i]);
        }
    }

#endif

    private string[] _toParse;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _toParse = new string[500];
        for (int i = 0; i < 500; i++)
        {
            _toParse[i] = Guid.NewGuid().ToString();
        }
    }
}
