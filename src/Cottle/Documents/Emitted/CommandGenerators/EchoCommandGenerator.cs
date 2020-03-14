namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class EchoCommandGenerator : ICommandGenerator
    {
        private readonly IExpressionGenerator _expression;

        public EchoCommandGenerator(IExpressionGenerator expression)
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