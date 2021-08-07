using System;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class SymbolExpressionGenerator : IExpressionGenerator
    {
        private readonly StoreMode _mode;
        private readonly Symbol _symbol;

        public SymbolExpressionGenerator(Symbol symbol, StoreMode mode)
        {
            _mode = mode;
            _symbol = symbol;
        }

        public void Generate(Emitter emitter)
        {
            switch (_mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadFrameGlobal();
                    emitter.EmitLoadInteger(_symbol.Index);
                    emitter.EmitLoadElementValueAtIndex<Value>();

                    break;

                case StoreMode.Local:
                    emitter.EmitLoadLocalValue(emitter.GetOrDeclareLocal(_symbol));

                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}