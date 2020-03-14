using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class AssignRenderCommandGenerator : ICommandGenerator
    {
        private readonly ICommandGenerator _body;
        private readonly Symbol _symbol;

        public AssignRenderCommandGenerator(Symbol symbol, ICommandGenerator body)
        {
            _body = body;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            // Prepare new buffer to store sub-rendering
            emitter.NewStringWriter();

            var buffer = emitter.DeclareLocalAndStore<StringWriter>();

            // Override output buffer, render body and retore previous buffer
            emitter.OutputEnqueue(buffer);

            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? (Local<bool>?)emitter.DeclareLocalAndStore<bool>() : null;

            emitter.OutputDequeue();

            // Render output buffer and store as local
            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadLocalReferenceAndRelease(buffer);
            emitter.InvokeStringWriterToString();
            emitter.NewStringValue();
            emitter.StoreReferenceAtIndex();

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.LoadLocalValueAndRelease(mayReturnCode.Value);

            return true;
        }
    }
}