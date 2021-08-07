using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators.Assign
{
    internal class RenderAssignStatementGenerator : AssignStatementGenerator
    {
        private readonly IStatementGenerator _body;

        public RenderAssignStatementGenerator(Symbol symbol, StoreMode mode, IStatementGenerator body) :
            base(symbol, mode)
        {
            _body = body;
        }

        public override bool Generate(Emitter emitter)
        {
            // Prepare new buffer to store sub-rendering
            emitter.EmitNewStringWriter();

            var buffer = emitter.EmitDeclareLocalAndStore<StringWriter>();

            // Override output buffer, render body and retore previous buffer
            emitter.OutputEnqueue(buffer);

            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? emitter.EmitDeclareLocalAndStore<bool>() : default;

            emitter.OutputDequeue();

            // Convert buffer to value and save as local variable
            emitter.EmitLoadLocalValueAndRelease(buffer);
            emitter.EmitCallStringWriterToString();
            emitter.EmitCallValueFromString();

            GenerateAssignment(emitter);

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.EmitLoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}