﻿using BenchmarkDotNet.Running;

namespace MiscUtil.Benchmarks
{
    class Program
    {
        public static void Main(string[] args) =>
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
