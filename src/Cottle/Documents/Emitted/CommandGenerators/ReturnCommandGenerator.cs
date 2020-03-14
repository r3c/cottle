namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class ReturnCommandGenerator : ICommandGenerator
    {
        private readonly IExpressionGenerator _expression;

        public ReturnCommandGenerator(IExpressionGenerator expression)
        {
            _expression = expression;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var result = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadResultAddress();
            emitter.LoadLocalReferenceAndRelease(result);
            emitter.StoreReferenceAtAddress();
            emitter.LoadBoolean(true);

            return true;
        }
    }
}