namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class DumpCommandGenerator : ICommandGenerator
    {
        private readonly IExpressionGenerator _expression;

        public DumpCommandGenerator(IExpressionGenerator expression)
        {
            _expression = expression;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var operand = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadOutput();
            emitter.LoadLocalReferenceAndRelease(operand);
            emitter.InvokeTextWriterWriteObject();

            return false;
        }
    }
}