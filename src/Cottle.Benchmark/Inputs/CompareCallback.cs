using System;

namespace Cottle.Benchmark.Inputs
{
    public record CompareCallback(Action Create, Func<string> Render);
}