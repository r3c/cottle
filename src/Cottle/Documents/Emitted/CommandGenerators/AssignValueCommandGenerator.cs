using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class AssignValueCommandGenerator : ICommandGenerator
    {
        private readonly IExpressionGenerator _expression;
        private readonly Symbol _symbol;

        public AssignValueCommandGenerator(Symbol symbol, IExpressionGenerator expression)
        {
            _expression = expression;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var value = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadLocalReferenceAndRelease(value);
            emitter.StoreReferenceAtIndex();

            return false;
        }
    }
}