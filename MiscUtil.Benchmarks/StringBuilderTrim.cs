using BenchmarkDotNet.Attributes;
using System;
using System.Text;

namespace MiscUtil.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public class StringBuilderTrim
    {
        private static readonly char[] s_trimChars = new[] { ' ', '[', '^', '#', '?', '!' };
        private const string s_str = "   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^###FROTZ [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^###FROTZ [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###   [[[^^^###   [[[^^^### [[[  [[[^^^###   [[[^^^###";
        private StringBuilder _sb;

        [Benchmark]
        public void TrimEndChars()
        {
            _sb.TrimEnd(s_trimChars);
        }

        [Benchmark]
        public void TrimStartChars()
        {
            _sb.TrimStart(s_trimChars);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _sb?.Clear();
            _sb = new StringBuilder(s_str);
        }
    }
}
