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
            emitter.LoadFrame();

            _expression.Generate(emitter);

            emitter.LoadOutput();
            emitter.InvokeFrameEcho();

            var value = emitter.DeclareLocalAndStore<string>();

            // Write string to output
            emitter.LoadOutput();
            emitter.LoadLocalValueAndRelease(value);
            emitter.InvokeTextWriterWriteString();

            return false;
        }
    }
}