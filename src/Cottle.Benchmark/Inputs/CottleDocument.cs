using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Documents;

namespace Cottle.Benchmark.Inputs
{
    public static class CottleDocument
    {
        public static readonly IReadOnlyList<Input<Func<string, IDocument>>> Inputs = new[]
        {
            new Input<Func<string, IDocument>>(
                Document.CreateDefault(TextReader.Null).DocumentOrThrow.GetType().Name,
                s => Document.CreateDefault(s).DocumentOrThrow),

            new Input<Func<string, IDocument>>(
                Document.CreateNative(TextReader.Null).DocumentOrThrow.GetType().Name,
                s => Document.CreateDefault(s).DocumentOrThrow),

#pragma warning disable 618
            new Input<Func<string, IDocument>>(nameof(SimpleDocument), s => new SimpleDocument(s))
#pragma warning restore 618
        };
    }
}