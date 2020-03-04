using System.Collections.Generic;

namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandCompositeGenerator : IGenerator
    {
        private readonly IReadOnlyList<IGenerator> _commands;

        public CommandCompositeGenerator(IReadOnlyList<IGenerator> commands)
        {
            _commands = commands;
        }

        public void Generate(Emitter emitter)
        {
            var exit = emitter.DeclareLabel();
            var halt = emitter.DeclareLabel();

            foreach (var command in _commands)
            {
                command.Generate(emitter);
                emitter.BranchIfTrue(halt);
            }

            emitter.LoadBoolean(false);
            emitter.BranchAlways(exit);

            emitter.MarkLabel(halt);
            emitter.LoadBoolean(true);

            emitter.MarkLabel(exit);
        }
    }
}