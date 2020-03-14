namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class AccessExpressionGenerator : IExpressionGenerator
    {
        private readonly IExpressionGenerator _source;
        private readonly IExpressionGenerator _subscript;

        public AccessExpressionGenerator(IExpressionGenerator source, IExpressionGenerator subscript)
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