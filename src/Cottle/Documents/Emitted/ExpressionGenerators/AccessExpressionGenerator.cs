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

            var source = emitter.EmitDeclareLocalAndStore<Value>();

            emitter.EmitLoadLocalAddressAndRelease(source);
            emitter.EmitCallValueFields();

            var fields = emitter.EmitDeclareLocalAndStore<IMap>();

            // Use subscript to get value from fields
            emitter.EmitLoadLocalValueAndRelease(fields);

            _subscript.Generate(emitter);

            var value = emitter.EmitDeclareLocalAndLoadAddress<Value>();

            emitter.EmitCallMapTryGet();

            var success = emitter.DeclareLabel();

            emitter.EmitBranchWhenTrue(success);

            // Emit undefined value on error
            emitter.EmitLoadUndefined();
            emitter.EmitStoreLocal(value);

            // Push value on stack
            emitter.MarkLabel(success);
            emitter.EmitLoadLocalValueAndRelease(value);
        }
    }
}