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
            emitter.EmitLoadSymbol(_symbol);
        }
    }
}