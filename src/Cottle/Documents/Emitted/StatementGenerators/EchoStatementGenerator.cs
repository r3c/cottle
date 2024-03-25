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
            // Convert subject to string
            _expression.Generate(emitter);

            var value = emitter.EmitDeclareLocalAndStore<Value>();

            emitter.EmitCallRuntimeEcho(value);

            var text = emitter.EmitDeclareLocalAndStore<string>();

            // Write string to output
            emitter.EmitLoadOutput();
            emitter.EmitLoadLocalValueAndRelease(text);
            emitter.EmitCallTextWriterWriteString();

            return false;
        }
    }
}