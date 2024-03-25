namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class WrapStatementGenerator : IStatementGenerator
    {
        private readonly IStatementGenerator _body;
        private readonly IExpressionGenerator _modifier;

        public WrapStatementGenerator(IExpressionGenerator modifier, IStatementGenerator body)
        {
            _body = body;
            _modifier = modifier;
        }

        public bool Generate(Emitter emitter)
        {
            _modifier.Generate(emitter);

            // Store modifier result as function
            var result = emitter.EmitDeclareLocalAndStore<Value>();

            emitter.EmitLoadLocalAddressAndRelease(result);
            emitter.EmitCallValueAsFunction();

            var modifier = emitter.EmitDeclareLocalAndStore<IFunction>();

            // Wrap with modifier
            emitter.EmitCallRuntimeWrap(modifier);

            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? emitter.EmitDeclareLocalAndStore<bool>() : default;

            // Unwrap by removing tail modifier
            emitter.EmitCallRuntimeUnwrap();
            emitter.EmitDiscard();

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.EmitLoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}