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
            emitter.EmitCallFrameUnwrap();

            var modifier = emitter.EmitDeclareLocalAndStore<IFunction>();

            // Generate body
            var mayReturn = _body.Generate(emitter);
            var mayReturnCode = mayReturn ? emitter.EmitDeclareLocalAndStore<bool>() : default;

            // Wrap with modifier backup
            emitter.EmitCallFrameWrap(modifier);

            // Forward return code to caller
            if (!mayReturn)
                return false;

            emitter.EmitLoadLocalValueAndRelease(mayReturnCode);

            return true;
        }
    }
}