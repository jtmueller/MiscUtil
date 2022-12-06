using BenchmarkDotNet.Attributes;

namespace MiscUtil.Benchmarks;

[Config(typeof(BenchmarkConfig))]
public class DecimalParsing
{
    [Benchmark(Baseline = true)]
    public void DecimalStringParse()
    {
        for (int i = 0; i < _toParse.Length; i++)
        {
            _ = double.TryParse(_toParse[i], out _);
        }
    }

    [Benchmark]
    public void DecimalSpanParse()
    {
        for (int i = 0; i < _toParse.Length; i++)
        {
            _ = _toParse[i].AsSpan().ToDecimal();
        }
    }

#if NET7_0_OR_GREATER

    [Benchmark]
    public void DecimalOptionParse()
    {
        for (int i = 0; i < _toParse.Length; i++)
        {
            _ = Option.Parse<decimal>(_toParse[i]);
        }
    }

#endif

    private string[] _toParse;
    private const int s_rand_seed = 1046527;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var random = new Random(s_rand_seed);
        _toParse = new string[500];
        for (int i = 0; i < 500; i++)
        {
            _toParse[i] = ((decimal)random.NextDouble() * random.Next(-10000, 10000)).ToString();
        }
    }
}
