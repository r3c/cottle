using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class AssignRenderStatementGenerator : IStatementGenerator
    {
        private readonly IStatementGenerator _body;
        private readonly Symbol _symbol;

        public AssignRenderStatementGenerator(Symbol symbol, IStatementGenerator body)
        {
            _body = body;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            // Prepare new buffer to store sub-rendering
            emitter.EmitNewStringWriter();

            var buffer = emitter.EmitDeclareLocalAndStore<StringWriter>();

            // Override output buffer, render body and retore previous buffer
            emitter.OutputEnqueue(buffer);

            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? emitter.EmitDeclareLocalAndStore<bool>() : default;

            emitter.OutputDequeue();

            // Render output buffer and store as local
            emitter.EmitLoadFrameSymbol(_symbol);
            emitter.EmitLoadLocalValueAndRelease(buffer);
            emitter.EmitCallStringWriterToString();
            emitter.EmitCallValueFromString();
            emitter.EmitStoreValueAtIndex<Value>();

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.EmitLoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}