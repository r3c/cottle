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

            var subject = emitter.DeclareLocalAndStore<Value>();

            // Convert subject to string
            emitter.LoadFrame();
            emitter.LoadLocalValueAndRelease(subject);
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