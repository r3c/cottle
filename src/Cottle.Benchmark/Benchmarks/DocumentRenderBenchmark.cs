using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Cottle.Benchmark.Inputs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace Cottle.Benchmark.Benchmarks
{
    public class DocumentRenderBenchmark
    {
        [ParamsSource(nameof(DocumentRenderBenchmark.GetConstructors))]
        public Input<Func<string, IDocument>> Constructor;

        [ParamsSource(nameof(DocumentRenderBenchmark.GetTemplates))]
        public Input<string> Template;

        public static IEnumerable<Input<Func<string, IDocument>>> GetConstructors => DocumentInput.Get();

        public static IEnumerable<Input<string>> GetTemplates => TemplateInput.Get();

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
        public void Execute()
        {
            _document.Render(DocumentRenderBenchmark.Context);
        }

        [GlobalSetup]
        public void Setup()
        {
            _document = Constructor.Value(Template.Value);
        }
    }
}