using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using Cottle.Benchmark.Inputs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace Cottle.Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class CompareBenchmark
    {
        [ParamsSource(nameof(CompareBenchmark.Engines))]
        public Input<Func<CompareCallback>> Engine;

        public static IEnumerable<Input<Func<CompareCallback>>> Engines => CompareEngine.GetInputs();

        private CompareCallback? _callback;

        [GlobalCleanup]
        public void Cleanup()
        {
            _callback = null;
        }

        [Benchmark]
        public void Create()
        {
            _callback?.Create();
        }

        [Benchmark]
        public void Render()
        {
            _callback?.Render();
        }

        [GlobalSetup]
        public void Setup()
        {
            var whitespaces = new Regex("\\s+");

            _callback = Engine.Value();

            var referenceString = whitespaces.Replace(CompareEngine.Reference, string.Empty);
            var rendererString = whitespaces.Replace(_callback.Render(), string.Empty);

            if (referenceString != rendererString)
                throw new InvalidOperationException($"Invalid output: '{rendererString}' expected '{referenceString}'");
        }
    }
}