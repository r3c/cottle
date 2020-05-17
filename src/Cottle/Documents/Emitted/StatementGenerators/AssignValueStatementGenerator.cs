using System;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class AssignValueStatementGenerator : IStatementGenerator
    {
        private readonly IExpressionGenerator _expression;
        private readonly Symbol _symbol;

        public AssignValueStatementGenerator(Symbol symbol, IExpressionGenerator expression)
        {
            _expression = expression;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var result = emitter.EmitDeclareLocalAndStore<Value>();

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

            return false;
        }
    }
}