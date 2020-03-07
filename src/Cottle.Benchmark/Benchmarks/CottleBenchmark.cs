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
        [ParamsSource(nameof(CottleBenchmark.Constructors))]
        public Input<Func<string, IDocument>> Constructor;

        [ParamsSource(nameof(CottleBenchmark.Templates))]
        public Input<string> Template;

        public static IEnumerable<Input<Func<string, IDocument>>> Constructors => CottleDocument.GetInputs();

        public static IEnumerable<Input<string>> Templates => CottleTemplate.GetInputs();

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
        public object Create()
        {
            return Constructor.Value(Template.Value);
        }

        [Benchmark]
        public object Render()
        {
            return _document.Render(CottleBenchmark.Context);
        }

        [GlobalSetup]
        public void Setup()
        {
            _document = Constructor.Value(Template.Value);
        }
    }
}