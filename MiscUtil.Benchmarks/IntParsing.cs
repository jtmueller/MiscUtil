using BenchmarkDotNet.Attributes;
using System;

namespace MiscUtil.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public class IntParsing
    {
        [Benchmark(Baseline = true)]
        public void IntStringParse()
        {
            for (int i = 0; i < _toParse.Length; i++)
            {
                int.TryParse(_toParse[i], out _);
            }
        }

        [Benchmark]
        public void IntSpanParse()
        {
            for (int i = 0; i < _toParse.Length; i++)
            {
                _toParse[i].AsSpan().ToInt32();
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
                _toParse[i] = random.Next(-10000, 10000).ToString();
            }
        }
    }
}
