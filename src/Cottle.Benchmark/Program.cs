using BenchmarkDotNet.Running;

namespace Cottle.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}