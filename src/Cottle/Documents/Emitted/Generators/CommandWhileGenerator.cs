namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandWhileGenerator : IGenerator
    {
        private readonly IGenerator _body;
        private readonly IGenerator _condition;

        public CommandWhileGenerator(IGenerator condition, IGenerator body)
        {
            _body = body;
            _condition = condition;
        }

        public void Generate(Emitter emitter)
        {
            // Branch to condition before first body execution
            var condition = emitter.DeclareLabel();

            emitter.BranchAlways(condition);

            // Execute loop command
            var loop = emitter.DeclareLabel();

            emitter.MarkLabel(loop);

            var halt = emitter.DeclareLabel();

            _body.Generate(emitter);

            emitter.BranchIfTrue(halt);

            // Evaluate loop condition
            emitter.MarkLabel(condition);

            _condition.Generate(emitter);

            // Restart loop if condition passed
            emitter.InvokeValueAsBoolean();
            emitter.BranchIfTrue(loop);

            // Otherwise exit loop with no-return flag
            var exit = emitter.DeclareLabel();

            emitter.LoadBoolean(false);
            emitter.BranchAlways(exit);

            // Exit loop with return flag
            emitter.MarkLabel(halt);
            emitter.LoadBoolean(true);

            // Exit command
            emitter.MarkLabel(exit);
        }
    }
}