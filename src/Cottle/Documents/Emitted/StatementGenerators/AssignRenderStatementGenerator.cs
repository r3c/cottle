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
            emitter.NewStringWriter();

            var buffer = emitter.DeclareLocalAndStore<StringWriter>();

            // Override output buffer, render body and retore previous buffer
            emitter.OutputEnqueue(buffer);

            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? (Local<bool>?)emitter.DeclareLocalAndStore<bool>() : null;

            emitter.OutputDequeue();

            // Render output buffer and store as local
            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadLocalValueAndRelease(buffer);
            emitter.InvokeStringWriterToString();
            emitter.InvokeValueFromString();
            emitter.StoreValueAtIndex<Value>();

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.LoadLocalValueAndRelease(mayReturnCode.Value);

            return true;
        }
    }
}