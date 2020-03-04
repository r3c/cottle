using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandAssignValueGenerator : IGenerator
    {
        private readonly IGenerator _expression;
        private readonly Symbol _symbol;

        public CommandAssignValueGenerator(Symbol symbol, IGenerator expression)
        {
            _expression = expression;
            _symbol = symbol;
        }

        public void Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            var value = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadLocalReferenceAndRelease(value);
            emitter.StoreReferenceAtIndex();
            emitter.LoadBoolean(false);
        }
    }
}