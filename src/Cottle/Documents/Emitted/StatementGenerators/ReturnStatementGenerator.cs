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
            emitter.EmitLoadResult();

            _expression.Generate(emitter);

            emitter.EmitStoreValueAtAddress<Value>();
            emitter.EmitLoadBoolean(true);

            return true;
        }
    }
}