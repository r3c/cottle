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
        public Input<Func<Func<Func<string>>>> Engine;

        public static IEnumerable<Input<Func<Func<Func<string>>>>> Engines => CompareEngine.GetInputs();

        private Func<Func<string>>? _constructor;
        private Func<string>? _renderer;

        [GlobalCleanup]
        public void Cleanup()
        {
            _constructor = null;
            _renderer = null;
        }

        [Benchmark]
        public object? Create()
        {
            return _constructor is not null ? _constructor() : null;
        }

        [Benchmark]
        public object? Render()
        {
            return _renderer is not null ? _renderer() : null;
        }

        [GlobalSetup]
        public void Setup()
        {
            var whitespaces = new Regex("\\s+");

            _constructor = Engine.Value();
            _renderer = _constructor();

            var referenceString = whitespaces.Replace(CompareEngine.Reference, string.Empty);
            var rendererString = whitespaces.Replace(_renderer(), string.Empty);

            if (referenceString != rendererString)
                throw new InvalidOperationException("Invalid output: " + rendererString);
        }
    }
}