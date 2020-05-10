namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class ReturnStatementGenerator : IStatementGenerator
    {
        private readonly IExpressionGenerator _expression;

        public ReturnStatementGenerator(IExpressionGenerator expression)
        {
            _expression = expression;
        }

        public bool Generate(Emitter emitter)
        {
            emitter.LoadResult();

            _expression.Generate(emitter);

            emitter.StoreValueAtAddress<Value>();
            emitter.LoadBoolean(true);

            return true;
        }
    }
}