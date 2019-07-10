using BenchmarkDotNet.Attributes;
using System;

namespace MiscUtil.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public class DoubleParsing
    {
        [Benchmark(Baseline = true)]
        public void DoubleStringParse()
        {
            for (int i = 0; i < _toParse.Length; i++)
            {
                double.TryParse(_toParse[i], out _);
            }
        }

        [Benchmark]
        public void DoubleSpanParse()
        {
            for (int i = 0; i < _toParse.Length; i++)
            {
                _toParse[i].AsSpan().ToDouble();
            }
        }

        private string[] _toParse;
        private const int s_rand_seed = 1046527;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(s_rand_seed);
            _toParse = new string[500];
            for (int i = 0; i < 500; i++)
            {
                _toParse[i] = (random.NextDouble() * random.Next(-10000, 10000)).ToString();
            }
        }
    }
}
