using System;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal abstract class AssignStatementGenerator : IStatementGenerator
    {
        private readonly StoreMode _mode;
        private readonly Symbol _symbol;

        protected AssignStatementGenerator(Symbol symbol, StoreMode mode)
        {
            _mode = mode;
            _symbol = symbol;
        }

        public abstract bool Generate(Emitter emitter);

        protected void GenerateAssignment(Emitter emitter)
        {
            var result = emitter.EmitDeclareLocalAndStore<Value>();

            switch (_mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadRuntimeGlobals();
                    emitter.EmitLoadInteger(_symbol.Index);
                    emitter.EmitLoadLocalValueAndRelease(result);
                    emitter.EmitStoreElementAtIndex<Value>();

                    break;

                case StoreMode.Local:
                    emitter.EmitLoadLocalValueAndRelease(result);
                    emitter.EmitStoreLocal(emitter.GetOrDeclareLocal(_symbol));

                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}