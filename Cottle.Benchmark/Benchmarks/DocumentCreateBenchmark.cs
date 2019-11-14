using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Cottle.Benchmark.Inputs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace Cottle.Benchmark.Benchmarks
{
    public class DocumentCreateBenchmark
    {
        [ParamsSource(nameof(DocumentCreateBenchmark.GetConstructors))]
        public Input<Func<string, IDocument>> Constructor;

        [ParamsSource(nameof(DocumentCreateBenchmark.GetTemplates))]
        public Input<string> Template;

        public static IEnumerable<Input<Func<string, IDocument>>> GetConstructors => DocumentInput.Get();

        public static IEnumerable<Input<string>> GetTemplates => TemplateInput.Get();

        [Benchmark]
        public void Execute()
        {
            Constructor.Value(Template.Value);
        }
    }
}