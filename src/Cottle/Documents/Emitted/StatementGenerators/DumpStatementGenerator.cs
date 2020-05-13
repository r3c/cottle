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

            var operand = emitter.EmitDeclareLocalAndStore<Value>();

            emitter.EmitLoadOutput();
            emitter.EmitLoadLocalAddressAndRelease(operand);
            emitter.EmitCallObjectToString();
            emitter.EmitCallTextWriterWriteObject();

            return false;
        }
    }
}