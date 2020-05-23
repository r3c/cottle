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
using Stubble.Compilation;

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

        public static IEnumerable<Input<Func<Func<Func<string>>>>> GetInputs()
        {
            // Render template with Cottle
            yield return new Input<Func<Func<Func<string>>>>(nameof(Cottle), () =>
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
                        ["description"] = p.Description,
                        ["name"] = p.Name,
                        ["price"] = p.Price
                    }).ToArray()
                });

                return () =>
                {
                    var document = Document.CreateDefault(template).DocumentOrThrow;

                    return () => document.Render(context);
                };
            });

            // Render template with DotLiquid
            yield return new Input<Func<Func<Func<string>>>>(nameof(DotLiquid), () =>
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
                    Filters = new[] { typeof(DotLiquidFilter) },
                    LocalVariables = Hash.FromAnonymousObject(new
                    {
                        products = CompareEngine.Products.Select(p => new Dictionary<string, object>
                        {
                            ["description"] = p.Description,
                            ["name"] = p.Name,
                            ["price"] = p.Price
                        }).ToArray()
                    })
                };

                return () =>
                {
                    var template = DotLiquid.Template.Parse(source);

                    return () => template.Render(renderParameters);
                };
            });

            // Render template with Fluid
            yield return new Input<Func<Func<Func<string>>>>(nameof(Fluid), () =>
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

                var options = new TemplateOptions();

                // Copy of "slice" filter with added missing safety check on length and no support for negative start
                options.Filters.AddFilter("safeslice", (input, arguments, _) =>
                {
                    var inputString = input.ToStringValue();
                    var rawStart = Convert.ToInt32(arguments.At(0).ToNumberValue());
                    var rawLength = Convert.ToInt32(arguments.At(1).ToNumberValue());

                    var safeStart = Math.Min(rawStart, inputString.Length);
                    var safeLength = Math.Min(rawLength, inputString.Length - safeStart);

                    return new StringValue(inputString.Substring(safeStart, safeLength));
                });

                // Formatting filter from floating point value to string
                options.Filters.AddFilter("tostring", (input, arguments, _) =>
                {
                    var format = arguments.At(0).ToStringValue();
                    var culture = CultureInfo.GetCultureInfo(arguments.At(1).ToStringValue());

                    return new StringValue(input.ToNumberValue().ToString(format, culture));
                });

                options.MemberAccessStrategy.Register<Product>(nameof(Product.Description), nameof(Product.Name), nameof(Product.Price));

                var context = new Fluid.TemplateContext(options);

                context.SetValue("products", CompareEngine.Products);

                return () =>
                {
                    var parser = new FluidParser();

                    if (!parser.TryParse(source, out var template, out var error))
                        throw new ArgumentOutOfRangeException(nameof(source), error);

                    return () => template.Render(context);
                };
            });

            // Render template with Mustachio
            yield return new Input<Func<Func<Func<string>>>>(nameof(Mustachio), () =>
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
                        ["name"] = p.Name,
                        ["price"] = p.Price.ToString("f1", CultureInfo.GetCultureInfo("en-US"))
                    }).ToArray()
                };

                return () =>
                {
                    var template = Mustachio.Parser.Parse(source);

                    return () => template(context);
                };
            });

            // Render template with RazorLight
            yield return new Input<Func<Func<Func<string>>>>(nameof(RazorLight), () =>
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
            });

            // Render template with Scriban
            yield return new Input<Func<Func<Func<string>>>>(nameof(Scriban), () =>
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
            });

            // Render template with Stubble
            yield return new Input<Func<Func<Func<string>>>>(nameof(Stubble), () =>
            {
                const string source = @"
<ul id='products'>
  {{#Products}}
    <li>
      <h2>{{Name}}</h2>
      <p>{{Description}} - Only {{Price}}$</p>
    </li>
  {{/Products}}
</ul>";

                // Similar to Mustachio, Stubble doesn't provide efficient render-time function calls so input data are
                // pre-rendered in model, introducing a bias in favor of Stubble in benchmark results.
                var context = new
                {
                    Products = CompareEngine.Products.Select(p => new
                    {
                        Description = p.Description.Substring(0, Math.Min(p.Description.Length, 15)),
                        Name = p.Name,
                        Price = p.Price.ToString("f1", CultureInfo.GetCultureInfo("en-US"))
                    }).ToArray()
                };

                // See: https://github.com/StubbleOrg/Stubble/blob/master/docs/compilation.md
                var stubble = new StubbleCompilationRenderer();

                return () =>
                {
                    var render = stubble.Compile(source, context);

                    return () => render(context);
                };
            });
        }

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