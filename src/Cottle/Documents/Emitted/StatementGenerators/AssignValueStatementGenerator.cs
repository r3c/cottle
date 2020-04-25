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

            var value = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadLocalValueAndRelease(value);
            emitter.StoreValueAtIndex<Value>();

            return false;
        }
    }
}