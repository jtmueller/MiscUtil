using BenchmarkDotNet.Attributes;
using System;
using System.Globalization;
using System.Text;

namespace MiscUtil.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public class ValueStringBuilding
    {
        [Benchmark(Baseline = true)]
        public void SB_ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < N; i++)
            {
                sb.Append(parts[i]);
                sb.Append(' ');
            }
            _ = sb.ToString();
        }

        [Benchmark]
        public void VSB_ToString()
        {
            var vsb = new ValueStringBuilder();
            for (int i = 0; i < N; i++)
            {
                vsb.Append(parts[i]);
                vsb.Append(' ');
            }
            _ = vsb.ToString();
        }

        [Benchmark]
        public void SB_NoString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < N; i++)
            {
                sb.Append(parts[i]);
                sb.Append(' ');
            }
        }

        [Benchmark]
        public void VSB_NoString()
        {
            using var vsb = new ValueStringBuilder();
            for (int i = 0; i < N; i++)
            {
                vsb.Append(parts[i]);
                vsb.Append(' ');
            }
        }

        [Params(1_000, 10_000)]
        public int N;

        private string[] parts;

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random(RAND_SEED);
            parts = new string[N];
            for (int i = 0; i < N; i++)
            {
                parts[i] = rand.Next().ToString("X8", CultureInfo.InvariantCulture);
            }
        }

        private const int RAND_SEED = 0xdead;
    }
}
