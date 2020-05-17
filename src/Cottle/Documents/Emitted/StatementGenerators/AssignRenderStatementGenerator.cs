using System;
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

            // Convert buffer to value and save as local variable
            emitter.EmitLoadLocalValueAndRelease(buffer);
            emitter.EmitCallStringWriterToString();
            emitter.EmitCallValueFromString();

            var result = emitter.EmitDeclareLocalAndStore<Value>();

            // Render output buffer and store as local
            switch (_symbol.Mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadFrameGlobal();
                    emitter.EmitLoadInteger(_symbol.Index);
                    emitter.EmitLoadLocalValueAndRelease(result);
                    emitter.EmitStoreElementAtIndex<Value>();

                    break;

                case StoreMode.Local:
                    emitter.EmitLoadLocalValueAndRelease(result);
                    emitter.EmitStoreLocal(emitter.GetOrDeclareSymbol(_symbol.Index));

                    break;

                default:
                    throw new InvalidOperationException();
            }

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.EmitLoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}