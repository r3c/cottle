namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class WhileStatementGenerator : IStatementGenerator
    {
        private readonly IStatementGenerator _body;
        private readonly IExpressionGenerator _condition;

        public WhileStatementGenerator(IExpressionGenerator condition, IStatementGenerator body)
        {
            _body = body;
            _condition = condition;
        }

        public bool Generate(Emitter emitter)
        {
            var start = emitter.DeclareLabel();

            // Evaluate loop condition
            emitter.MarkLabel(start);

            _condition.Generate(emitter);

            // Terminate loop if condition failed
            var condition = emitter.DeclareLocalAndStore<Value>();
            var exitRegular = emitter.DeclareLabel();

            emitter.LoadLocalAddressAndRelease(condition);
            emitter.InvokeValueAsBoolean();
            emitter.BranchIfFalse(exitRegular);

            // Execute loop statement
            var mayReturn = _body.Generate(emitter);

            // Exit loop following return statement from body
            var exitReturn = emitter.DeclareLabel();

            if (mayReturn)
            {
                emitter.LoadDuplicate();
                emitter.BranchIfTrue(exitReturn);
                emitter.Discard();
            }

            // Restart loop
            emitter.BranchAlways(start);

            // End of loop
            emitter.MarkLabel(exitRegular);

            if (mayReturn)
                emitter.LoadBoolean(false);

            // Exit statement
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}