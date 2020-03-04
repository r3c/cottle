using System.Collections.Generic;

namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandIfGenerator : IGenerator
    {
        private readonly IReadOnlyList<KeyValuePair<IGenerator, IGenerator>> _branches;
        private readonly IGenerator _fallback;

        public CommandIfGenerator(IReadOnlyList<KeyValuePair<IGenerator, IGenerator>> branches, IGenerator fallback)
        {
            _branches = branches;
            _fallback = fallback;
        }

        public void Generate(Emitter emitter)
        {
            var exit = emitter.DeclareLabel();

            // Emit conditional branches
            foreach (var branch in _branches)
            {
                // Evaluate branch condition, jump to next branch if false
                branch.Key.Generate(emitter);

                emitter.InvokeValueAsBoolean();

                var next = emitter.DeclareLabel();

                emitter.BranchIfFalse(next);

                // Execute branch body and jump over next branches
                branch.Value.Generate(emitter);

                emitter.BranchAlways(exit);
                emitter.MarkLabel(next);
            }

            // Emit fallback branch if any or null value otherwise
            if (_fallback != null)
                _fallback.Generate(emitter);
            else
                emitter.LoadBoolean(false);

            // Mark end of statement
            emitter.MarkLabel(exit);
        }
    }
}