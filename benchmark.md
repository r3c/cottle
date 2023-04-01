---
layout: default
title: Cottle - Benchmark
---

# Methodology

## Benchmark contents

This page contains parsing and rendering benchmark of Cottle compared to other
template engines, using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)
library.

**Parsing** benchmark measures the time needed to transform a template stored
as a string into an in-memory object that can be passed input data for
rendering. This part is usually the most time consuming, which is why result
should be cached by client application.

**Rendering** benchmark measures the time needed to render a parsed template
into a string output, taking variable input data at render time.

Same template source code was used for both benchmarks and adapted to every
template engine library. Here is the Cottle version of this source code:

```sh
<ul id='products'>
  {for product in products:
    <li>
      <h2>{product.name}</h2>
      <p>{slice(product.description, 0, 15)} - Only {format(product.price, "n:f1", "en-US")}$</p>
    </li>
  }
</ul>
```

Template is then rendered with an input context built with an array of 5 items
as the `products` variable. Full benchmark source code can be found
[here](https://github.com/r3c/cottle/blob/master/src/Cottle.Benchmark/Inputs/CompareEngine.cs).

## Template engines

- Cottle v2.0.9 (see discussion [about Cottle](#about-cottle))
- DotLiquid v2.2.692
- Fluid v2.4.0
- Mustachio v2.1.0 (see discussion [about Mustachio](#about-mustachio))
- RazorLight v2.3.1
- Scriban v5.7.0

# Result

## Benchmark configuration

```
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2728/21H2/November2021Update)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
```

## Benchmark scores

Parsing and rendering benchmark, scores are expressed in microseconds per
operation.

<div style="display: flex; flex-wrap: wrap; justify-content: space-evenly;">
    <canvas id="create" width="400" height="480"></canvas>
    <canvas id="render" width="400" height="480"></canvas>
</div>
<script type="text/javascript" src="https://cdn.jsdelivr.net/npm/chart.js@3.7.1/dist/chart.min.js" async></script>
<script type="text/javascript">
    window.addEventListener('load', function () {
        // Paste last line of `./benchmark.sh` output below
        var benchmarks = {"Cottle":{"create":7589,"render":2932},"DotLiquid":{"create":13724,"render":67900},"Fluid":{"create":5540,"render":3605},"Mustachio":{"create":20315,"render":15576},"RazorLight":{"create":53162,"render":809517},"Scriban":{"create":25454,"render":724053}};

        // https://mika-s.github.io/javascript/colors/hsl/2017/12/05/generating-random-colors-in-javascript.html
        var generateHslaColors = (saturation, lightness, alpha, amount, shift) => {
            var colors = [];
            var step = Math.trunc(360 / amount);

            for (var i = 0; i < amount; i++) {
                colors.push(`hsla(${(i * step + shift * 360) % 360},${saturation}%,${lightness}%,${alpha})`);
            }

            return colors;
        };

        // Build charts
        var charts = [{
            element: document.getElementById('create'),
            extract: b => b.create,
            label: 'Parsing time',
            shift: 0
        }, {
            element: document.getElementById('render'),
            extract: b => b.render,
            label: 'Rendering time',
            shift: 0.05
        }];

        charts.forEach(chart => new Chart(chart.element.getContext('2d'), {
            type: 'bar',
            data: {
                labels: Object.keys(benchmarks),
                datasets: [{
                    label: chart.label,
                    data: Object.values(benchmarks).map(chart.extract),
                    backgroundColor: generateHslaColors(50, 75, 1.0, Object.keys(benchmarks).length, chart.shift),
                    borderColor: generateHslaColors(25, 75, 1.0, Object.keys(benchmarks).length, chart.shift),
                    borderWidth: 2
                }]
            },
            options: {
                responsive: false,
                scales: {
                    y: {
    					beginAtZero: true,
    					ticks: {
    						callback: function (v) {
    							return v + ' µs';
    						}
    					},
    					type: 'logarithmic'
                    }
                }
            }
        }));
    });

</script>

Other libraries were considered in this benchmark but not included as their
performance results are too far away and would impact graph scale too much:

- RazorEngineCore v2022.1.2 (render time is over 6000% of Cottle, see #149)

# Discussion

## About Cottle

Cottle results were measured using default document compiler (method
`Document.CreateDefault`) &
[no optimizer](https://cottle.readthedocs.io/en/stable/page/04-configuration.html#optimizer-deactivation)
for compilation benchmark, and
[native](https://cottle.readthedocs.io/en/stable/page/05-advanced.html#native-documents)
document compiler (method `Document.CreateNative`) for rendering benchmark to
maximize performances.

## About Mustachio

Function calls could not be adapted to Mustachio benchmark due to the
logic-less approach of the library. These functions were however required for
formatting price floating point values to strings, so source code for Mustachio
is not a 1:1 equivalent to the reference version. Formatting was pre-applied in
C# code instead, resulting in an artificial slight performance boost in favor
of Mustachio due to this processing not being considered as part of the
measurement.
