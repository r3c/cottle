using System;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class AssignValueStatementGenerator : IStatementGenerator
    {
        private readonly IExpressionGenerator _expression;
        private readonly StoreMode _mode;
        private readonly Symbol _symbol;

        public AssignValueStatementGenerator(Symbol symbol, StoreMode mode, IExpressionGenerator expression)
        {
            _expression = expression;
            _mode = mode;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var result = emitter.EmitDeclareLocalAndStore<Value>();

            switch (_mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadFrameGlobal();
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

            return false;
        }
    }
}