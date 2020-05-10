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

            var source = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadLocalAddressAndRelease(source);
            emitter.InvokeValueFields();

            var fields = emitter.DeclareLocalAndStore<IMap>();

            // Use subscript to get value from fields
            emitter.LoadLocalValueAndRelease(fields);

            _subscript.Generate(emitter);

            var value = emitter.DeclareLocalAndLoadAddress<Value>();

            emitter.InvokeMapTryGet();

            var success = emitter.DeclareLabel();

            emitter.BranchIfTrue(success);

            // Emit undefined value on error
            emitter.LoadUndefined();
            emitter.StoreLocal(value);

            // Push value on stack
            emitter.MarkLabel(success);
            emitter.LoadLocalValueAndRelease(value);
        }
    }
}