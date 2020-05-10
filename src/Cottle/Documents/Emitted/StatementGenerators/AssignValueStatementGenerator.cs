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
            emitter.LoadFrameSymbol(_symbol);

            _expression.Generate(emitter);

            emitter.StoreValueAtIndex<Value>();

            return false;
        }
    }
}