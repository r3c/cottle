using System.Collections.Generic;

namespace Cottle.Benchmark.Inputs
{
    public static class CottleTemplate
    {
        public static readonly IReadOnlyList<Input<string>> Inputs = new[]
        {
            new Input<string>("empty template", string.Empty),

            new Input<string>("simple interpolation", "Hello {a} world {b}!"),

            new Input<string>("factorial", @"
                {set factorial(n) to:
                    {if n > 1:
                        {return n * factorial(n - 1)}
                    |else:
                        {return 1}
                    }
                }
                {return factorial(5)}"),

            new Input<string>("hanoi towers", @"
                {set hanoi_rec(n, from, by, to) to:
                    {set n to n - 1}
                    {if n > 0:
                        {hanoi_rec(n, from, to, by)}
                    }
                    {from} -> {to}, 
                    {if n > 0:
                        {hanoi_rec(n, by, from, to)}
                    }
                }
                {set hanoi(n) to:
                    {hanoi_rec(n, 'A', 'B', 'C')}
                }
                {hanoi(3)}")
        };
    }
}