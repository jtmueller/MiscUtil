using BenchmarkDotNet.Attributes;
using System;

namespace MiscUtil.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public class GuidParsing
    {
        [Benchmark(Baseline = true)]
        public void GuidStringParse()
        {
            for (int i = 0; i < _toParse.Length; i++)
            {
                Guid.TryParse(_toParse[i], out _);
            }
        }

        [Benchmark]
        public void GuidSpanParse()
        {
            for (int i = 0; i < _toParse.Length; i++)
            {
                _toParse[i].AsSpan().ToGuid();
            }
        }

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
}
