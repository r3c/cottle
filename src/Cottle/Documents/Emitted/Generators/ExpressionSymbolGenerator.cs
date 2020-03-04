using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.Generators
{
    internal class ExpressionSymbolGenerator : IGenerator
    {
        private readonly Symbol _symbol;

        public ExpressionSymbolGenerator(Symbol symbol)
        {
            _symbol = symbol;
        }

        public void Generate(Emitter emitter)
        {
            emitter.LoadSymbol(_symbol);
        }
    }
}