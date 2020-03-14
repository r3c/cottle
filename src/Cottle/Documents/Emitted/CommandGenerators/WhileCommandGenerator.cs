namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class WhileCommandGenerator : ICommandGenerator
    {
        private readonly ICommandGenerator _body;
        private readonly IExpressionGenerator _condition;

        public WhileCommandGenerator(IExpressionGenerator condition, ICommandGenerator body)
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
            var exitRegular = emitter.DeclareLabel();

            emitter.InvokeValueAsBoolean();
            emitter.BranchIfFalse(exitRegular);

            // Execute loop command
            var mayReturn = _body.Generate(emitter);

            // Exit loop following return command from body
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

            // Exit command
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}