namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class DumpStatementGenerator : IStatementGenerator
    {
        private readonly IExpressionGenerator _expression;

        public DumpStatementGenerator(IExpressionGenerator expression)
        {
            _expression = expression;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var operand = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadOutput();
            emitter.LoadLocalAddressAndRelease(operand);
            emitter.InvokeObjectToString();
            emitter.InvokeTextWriterWriteObject();

            return false;
        }
    }
}