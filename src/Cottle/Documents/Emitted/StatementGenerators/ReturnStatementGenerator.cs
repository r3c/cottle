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
            _expression.Generate(emitter);

            var result = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadResult();
            emitter.LoadLocalValueAndRelease(result);
            emitter.StoreValueAtAddress<Value>();
            emitter.LoadBoolean(true);

            return true;
        }
    }
}