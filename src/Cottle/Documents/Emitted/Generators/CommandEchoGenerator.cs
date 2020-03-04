namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandEchoGenerator : IGenerator
    {
        private readonly IGenerator _expression;

        public CommandEchoGenerator(IGenerator expression)
        {
            _expression = expression;
        }

        public void Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var operand = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadOutput();
            emitter.LoadLocalReferenceAndRelease(operand);
            emitter.InvokeValueAsString();
            emitter.InvokeTextWriterWriteString();
            emitter.LoadBoolean(false);
        }
    }
}