namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandDumpGenerator : IGenerator
    {
        private readonly IGenerator _expression;

        public CommandDumpGenerator(IGenerator expression)
        {
            _expression = expression;
        }

        public void Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var operand = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadOutput();
            emitter.LoadLocalReferenceAndRelease(operand);
            emitter.InvokeTextWriterWriteObject();
            emitter.LoadBoolean(false);
        }
    }
}