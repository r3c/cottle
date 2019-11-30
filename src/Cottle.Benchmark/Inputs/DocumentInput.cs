using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Documents;

namespace Cottle.Benchmark.Inputs
{
    public static class DocumentInput
    {
        public static IEnumerable<Input<Func<string, IDocument>>> Get()
        {
            yield return new Input<Func<string, IDocument>>(
                Document.CreateDefault(TextReader.Null).DocumentOrThrow.GetType().Name,
                s => Document.CreateDefault(s).DocumentOrThrow);

            yield return new Input<Func<string, IDocument>>(
                Document.CreateNative(TextReader.Null).DocumentOrThrow.GetType().Name,
                s => Document.CreateDefault(s).DocumentOrThrow);

            yield return new Input<Func<string, IDocument>>(nameof(SimpleDocument), s => new SimpleDocument(s));
        }
    }
}