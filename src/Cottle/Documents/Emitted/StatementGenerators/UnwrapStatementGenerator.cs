namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class UnwrapStatementGenerator : IStatementGenerator
    {
        private readonly IStatementGenerator _body;

        public UnwrapStatementGenerator(IStatementGenerator body)
        {
            _body = body;
        }

        public bool Generate(Emitter emitter)
        {
            // Unwrap and backup tail modifier
            emitter.LoadFrame();
            emitter.InvokeFrameUnwrap();

            var modifier = emitter.DeclareLocalAndStore<IFunction>();

            // Generate body
            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? emitter.DeclareLocalAndStore<bool>() : default;

            // Wrap with modifier backup
            emitter.LoadFrame();
            emitter.LoadLocalValueAndRelease(modifier);
            emitter.InvokeFrameWrap();

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.LoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}