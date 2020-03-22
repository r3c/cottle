namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class EchoStatementGenerator : IStatementGenerator
    {
        private readonly IExpressionGenerator _expression;

        public EchoStatementGenerator(IExpressionGenerator expression)
        {
            _expression = expression;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var operand = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadOutput();
            emitter.LoadLocalReferenceAndRelease(operand);
            emitter.InvokeValueAsString();
            emitter.InvokeTextWriterWriteString();

            return false;
        }
    }
}