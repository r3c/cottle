using System;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class SymbolExpressionGenerator : IExpressionGenerator
    {
        private readonly Symbol _symbol;

        public SymbolExpressionGenerator(Symbol symbol)
        {
            _symbol = symbol;
        }

        public void Generate(Emitter emitter)
        {
            switch (_symbol.Mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadFrameGlobal();
                    emitter.EmitLoadInteger(_symbol.Index);
                    emitter.EmitLoadElementValueAtIndex<Value>();

                    break;

                case StoreMode.Local:
                    emitter.EmitLoadLocalValue(emitter.GetOrDeclareSymbol(_symbol.Index));

                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}