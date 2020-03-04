namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandReturnGenerator : IGenerator
    {
        private readonly IGenerator _expression;

        public CommandReturnGenerator(IGenerator expression)
        {
            _expression = expression;
        }

        public void Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var result = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadResultAddress();
            emitter.LoadLocalReferenceAndRelease(result);
            emitter.StoreReferenceAtAddress();
            emitter.LoadBoolean(true);
        }
    }
}