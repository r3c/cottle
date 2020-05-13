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
            emitter.EmitLoadFrame();

            _expression.Generate(emitter);

            emitter.EmitLoadOutput();
            emitter.EmitCallFrameEcho();

            var value = emitter.EmitDeclareLocalAndStore<string>();

            // Write string to output
            emitter.EmitLoadOutput();
            emitter.EmitLoadLocalValueAndRelease(value);
            emitter.EmitCallTextWriterWriteString();

            return false;
        }
    }
}