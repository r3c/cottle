using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandAssignRenderGenerator : IGenerator
    {
        private readonly IGenerator _body;
        private readonly Symbol _symbol;

        public CommandAssignRenderGenerator(Symbol symbol, IGenerator body)
        {
            _body = body;
            _symbol = symbol;
        }

        public void Generate(Emitter emitter)
        {
            // Prepare new buffer to store sub-rendering
            emitter.NewStringWriter();

            var buffer = emitter.DeclareLocalAndStore<StringWriter>();

            // Override output buffer, render body and retore previous buffer
            emitter.OutputEnqueue(buffer);

            _body.Generate(emitter);

            emitter.Discard();
            emitter.OutputDequeue();

            // Render output buffer and store as local
            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadLocalReferenceAndRelease(buffer);
            emitter.InvokeStringWriterToString();
            emitter.NewStringValue();
            emitter.StoreReferenceAtIndex();
            emitter.LoadBoolean(false);
        }
    }
}