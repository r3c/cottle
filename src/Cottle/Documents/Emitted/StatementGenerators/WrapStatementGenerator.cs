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
            var result = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadLocalAddressAndRelease(result);
            emitter.InvokeValueAsFunction();

            var modifier = emitter.DeclareLocalAndStore<IFunction>();

            // Wrap with modifier
            emitter.LoadFrame();
            emitter.LoadLocalValueAndRelease(modifier);
            emitter.InvokeFrameWrap();

            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? emitter.DeclareLocalAndStore<bool>() : default;

            // Unwrap by removing tail modifier
            emitter.LoadFrame();
            emitter.InvokeFrameUnwrap();
            emitter.Discard();

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.LoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}