using System.Collections.Generic;

namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class CompositeCommandGenerator : ICommandGenerator
    {
        private readonly IReadOnlyList<ICommandGenerator> _commands;

        public CompositeCommandGenerator(IReadOnlyList<ICommandGenerator> commands)
        {
            _commands = commands;
        }

        public bool Generate(Emitter emitter)
        {
            var exitReturn = emitter.DeclareLabel();
            var mayReturn = false;

            foreach (var command in _commands)
            {
                if (!command.Generate(emitter))
                    continue;

                emitter.LoadDuplicate();
                emitter.BranchIfTrue(exitReturn);
                emitter.Discard();

                mayReturn = true;
            }

            if (mayReturn)
                emitter.LoadBoolean(false);

            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}