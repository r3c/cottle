namespace Cottle.Documents.Emitted.Generators
{
    internal class ExpressionAccessGenerator : IGenerator
    {
        private readonly IGenerator _source;
        private readonly IGenerator _subscript;

        public ExpressionAccessGenerator(IGenerator source, IGenerator subscript)
        {
            _source = source;
            _subscript = subscript;
        }

        public void Generate(Emitter emitter)
        {
            // Evaluate source expression and access fields
            _source.Generate(emitter);

            emitter.InvokeValueFields();

            var fields = emitter.DeclareLocalAndStore<IMap>();

            // Evaluate subscript expression
            _subscript.Generate(emitter);

            var subscript = emitter.DeclareLocalAndStore<Value>();

            // Use subscript to get value from fields
            emitter.LoadLocalReferenceAndRelease(fields);
            emitter.LoadLocalReferenceAndRelease(subscript);

            var value = emitter.DeclareLocalAndLoadAddress<Value>();

            emitter.InvokeMapTryGet();

            var success = emitter.DeclareLabel();

            emitter.BranchIfTrue(success);

            // Emit void value on error
            emitter.LoadVoid();
            emitter.StoreLocal(value);

            // Push value on stack
            emitter.MarkLabel(success);
            emitter.LoadLocalReferenceAndRelease(value);
        }
    }
}