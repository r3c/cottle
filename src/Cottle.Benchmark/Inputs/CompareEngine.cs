using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DotLiquid;
using Fluid;
using Fluid.Values;
using RazorLight;
using RazorLight.Razor;
using Scriban;
using Scriban.Runtime;

// ReSharper disable MemberCanBePrivate.Global

namespace Cottle.Benchmark.Inputs
{
    /// <summary>
    /// Input class used to benchmark parsing and rendering performance of several templating engine. Code adapted from
    /// Scriban library: https://github.com/lunet-io/scriban
    /// </summary>
    public static class CompareEngine
    {
        /// <summary>
        /// Template result reference.
        /// </summary>
        public const string Reference = @"
<ul id='products'>
    <li>
        <h2>Product 0</h2>
        <p>Description # - Only 0.0$</p>
    </li>
    <li>
        <h2>Product 1</h2>
        <p>Description ## - Only 0.3$</p>
    </li>
    <li>
        <h2>Product 2</h2>
        <p>Description ### - Only 0.6$</p>
    </li>
    <li>
        <h2>Product 3</h2>
        <p>Description ### - Only 0.9$</p>
    </li>
    <li>
        <h2>Product 4</h2>
        <p>Description ### - Only 1.2$</p>
    </li>
</ul>";

        public static IEnumerable<Input<Func<Func<Func<string>>>>> Inputs => new[]
        {
            // Render template with Cottle
            new Input<Func<Func<Func<string>>>>(nameof(Cottle), () =>
            {
                const string template = @"
<ul id='products'>
  {for product in products:
    <li>
      <h2>{product.name}</h2>
      <p>{slice(product.description, 0, 15)} - Only {format(product.price, ""n:f1"", ""en-US"")}$</p>
    </li>
  }
</ul>";

                var context = Context.CreateBuiltin(new Dictionary<Value, Value>
                {
                    ["products"] = CompareEngine.Products.Select(p => (Value)new Dictionary<Value, Value>
                    {
                        ["description"] = p.Description, ["name"] = p.Name, ["price"] = p.Price
                    }).ToArray()
                });

                return () =>
                {
                    var document = Document.CreateDefault(template).DocumentOrThrow;

                    return () => document.Render(context);
                };
            }),

            // Render template with DotLiquid
            new Input<Func<Func<Func<string>>>>(nameof(DotLiquid), () =>
            {
                const string source = @"
<ul id='products'>
  {% for product in products %}
    <li>
      <h2>{{ product.name }}</h2>
      <p>{{ product.description | slice: 0, 15 }} - Only {{ product.price | tostring: ""f1"", ""en-US"" }}$</p>
    </li>
  {% endfor %}
</ul>";

                var renderParameters = new RenderParameters(CultureInfo.InvariantCulture)
                {
                    Filters = new[] { typeof(DotLiquidFilter) }, LocalVariables = Hash.FromAnonymousObject(new
                    {
                        products = CompareEngine.Products.Select(p => new Dictionary<string, object>
                        {
                            ["description"] = p.Description, ["name"] = p.Name, ["price"] = p.Price
                        }).ToArray()
                    })
                };

                return () =>
                {
                    var template = DotLiquid.Template.Parse(source);

                    return () => template.Render(renderParameters);
                };
            }),

            // Render template with Fluid
            new Input<Func<Func<Func<string>>>>(nameof(Fluid), () =>
            {
                const string source = @"
<ul id='products'>
  {% for product in products %}
    <li>
      <h2>{{ product.Name }}</h2>
      <p>{{ product.Description | safeslice: 0, 15 }} - Only {{ product.Price | tostring: ""f1"", ""en-US"" }}$</p>
    </li>
  {% endfor %}
</ul>";

                var context = new Fluid.TemplateContext();

                // Copy of "slice" filter with added missing safety check on length and no support for negative start
                context.Filters.AddFilter("safeslice", (input, arguments, _) =>
                {
                    var inputString = input.ToStringValue();
                    var rawStart = Convert.ToInt32(arguments.At(0).ToNumberValue());
                    var rawLength = Convert.ToInt32(arguments.At(1).ToNumberValue());

                    var safeStart = Math.Min(rawStart, inputString.Length);
                    var safeLength = Math.Min(rawLength, inputString.Length - safeStart);

                    return new StringValue(inputString.Substring(safeStart, safeLength));
                });

                // Formatting filter from floating point value to string
                context.Filters.AddFilter("tostring", (input, arguments, _) =>
                {
                    var format = arguments.At(0).ToStringValue();
                    var culture = CultureInfo.GetCultureInfo(arguments.At(1).ToStringValue());

                    return new StringValue(input.ToNumberValue().ToString(format, culture));
                });

                context.MemberAccessStrategy.Register<Product>();
                context.SetValue("products", CompareEngine.Products);

                return () =>
                {
                    if (!BaseFluidTemplate<FluidTemplate>.TryParse(source, out var template, out var messages))
                        throw new ArgumentOutOfRangeException(nameof(source), string.Join(", ", messages));

                    return () => template.Render(context);
                };
            }),

            // Render template with Mustachio
            new Input<Func<Func<Func<string>>>>(nameof(Mustachio), () =>
            {
                const string source = @"
<ul id='products'>
  {{#each products}}
    <li>
      <h2>{{name}}</h2>
      <p>{{description}} - Only {{price}}$</p>
    </li>
  {{/each}}
</ul>";

                // Mustachio doesn't support function calls so input data are pre-rendered in model, which is induces
                // an artificial performance boost in benchmark result compared to other libraries.
                var context = new Dictionary<string, object>
                {
                    ["products"] = CompareEngine.Products.Select(p => new Dictionary<string, object>
                    {
                        ["description"] = p.Description.Substring(0, Math.Min(p.Description.Length, 15)),
                        ["name"] = p.Name, ["price"] = p.Price.ToString("f1", CultureInfo.GetCultureInfo("en-US"))
                    }).ToArray()
                };

                return () =>
                {
                    var template = Mustachio.Parser.Parse(source);

                    return () => template(context);
                };
            }),

            // Render template with RazorLight
            new Input<Func<Func<Func<string>>>>(nameof(RazorLight), () =>
            {
                const string content = @"
<ul id='products'>
  @foreach (var product in Model.Products)
  {
    <li>
      <h2>@product.Name</h2>
      <p>@product.Description.Substring(0, System.Math.Min(product.Description.Length, 15)) - Only @product.Price.ToString(""f1"", System.Globalization.CultureInfo.CreateSpecificCulture(""en-US""))$</p>
    </li>
  }
</ul>";

                var context = new { CompareEngine.Products };

                return () =>
                {
                    var engine = new RazorLightEngineBuilder().UseProject(new StringRazorLightProject(content)).Build();

                    return () => engine.CompileRenderAsync(string.Empty, context).Result;
                };
            }),

            // Render template with Scriban
            new Input<Func<Func<Func<string>>>>(nameof(Scriban), () =>
            {
                const string source = @"
<ul id='products'>
  {{ for product in products }}
    <li>
      <h2>{{ product.name }}</h2>
      <p>{{ product.description | string.slice1 0 15 }} - Only {{ product.price | math.format ""f1"" ""en-US"" }}$</p>
    </li>
  {{ end }}
</ul>";

                var context = new LiquidTemplateContext();

                context.PushGlobal(new ScriptObject
                {
                    {
                        "products", CompareEngine.Products.Select(p => new ScriptObject
                        {
                            ["description"] = p.Description, ["name"] = p.Name, ["price"] = p.Price
                        })
                    }
                });

                return () =>
                {
                    var template = Scriban.Template.Parse(source);

                    return () => template.Render(context);
                };
            }),
        };

        private static readonly IReadOnlyList<Product> Products = Enumerable.Range(0, 5).Select(i => new Product
                { Description = "Description " + new string('#', i + 1), Name = $"Product {i}", Price = i * 0.3f })
            .ToList();

        public struct Product
        {
            public string Description;
            public float Price;
            public string Name;
        }

        public static class DotLiquidFilter
        {
            public static string Tostring(float input, string format, string culture)
            {
                return input.ToString(format, CultureInfo.GetCultureInfo(culture));
            }
        }

        private class StringRazorLightProject : RazorLightProject
        {
            private readonly string _content;

            public StringRazorLightProject(string content)
            {
                _content = content;
            }

            public override Task<RazorLightProjectItem> GetItemAsync(string templateKey)
            {
                return Task.FromResult(new TextSourceRazorProjectItem(templateKey, _content) as RazorLightProjectItem);
            }

            public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
            {
                return Task.FromResult(Enumerable.Empty<RazorLightProjectItem>());
            }
        }
    }
}