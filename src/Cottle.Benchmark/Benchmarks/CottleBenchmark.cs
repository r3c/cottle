using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Cottle.Benchmark.Inputs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace Cottle.Benchmark.Benchmarks
{
    public class CottleBenchmark
    {
        [ParamsSource(nameof(CottleBenchmark.GetConstructors))]
        public Input<Func<string, IDocument>> Constructor;

        [ParamsSource(nameof(CottleBenchmark.GetTemplates))]
        public Input<string> Template;

        public static IEnumerable<Input<Func<string, IDocument>>> GetConstructors => CottleDocument.Inputs;

        public static IEnumerable<Input<string>> GetTemplates => CottleTemplate.Inputs;

        private IDocument _document;

        private static readonly IContext Context = Cottle.Context.CreateBuiltin(new Dictionary<Value, Value>
        {
            ["a"] = 1,
            ["b"] = 2
        });

        [GlobalCleanup]
        public void Cleanup()
        {
            _document = null;
        }

        [Benchmark]
        public void Create()
        {
            Constructor.Value(Template.Value);
        }

        [Benchmark]
        public void Render()
        {
            _document.Render(CottleBenchmark.Context);
        }

        [GlobalSetup]
        public void Setup()
        {
            _document = Constructor.Value(Template.Value);
        }
    }
}