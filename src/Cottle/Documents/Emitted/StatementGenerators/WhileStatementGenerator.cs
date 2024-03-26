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
            var condition = emitter.EmitDeclareLocalAndStore<Value>();
            var exitRegular = emitter.DeclareLabel();

            emitter.EmitLoadLocalAddressAndRelease(condition);
            emitter.EmitCallValueAsBoolean();
            emitter.EmitBranchWhenFalse(exitRegular);

            // Execute loop statement
            emitter.EmitRuntimeTick();

            var mayReturn = _body.Generate(emitter);

            // Exit loop following return statement from body
            var exitReturn = emitter.DeclareLabel();

            if (mayReturn)
            {
                emitter.EmitLoadDuplicate();
                emitter.EmitBranchWhenTrue(exitReturn);
                emitter.EmitDiscard();
            }

            // Restart loop
            emitter.EmitBranchAlways(start);

            // End of loop
            emitter.MarkLabel(exitRegular);

            if (mayReturn)
                emitter.EmitLoadBoolean(false);

            // Exit statement
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}