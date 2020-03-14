using System.Collections.Generic;

namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class IfCommandGenerator : ICommandGenerator
    {
        private readonly IReadOnlyList<KeyValuePair<IExpressionGenerator, ICommandGenerator>> _branches;
        private readonly ICommandGenerator _fallback;

        public IfCommandGenerator(IReadOnlyList<KeyValuePair<IExpressionGenerator, ICommandGenerator>> branches,
            ICommandGenerator fallback)
        {
            _branches = branches;
            _fallback = fallback;
        }

        public bool Generate(Emitter emitter)
        {
            var exitRegular = emitter.DeclareLabel();
            var exitReturn = emitter.DeclareLabel();
            var mayReturn = false;

            // Emit conditional branches
            foreach (var branch in _branches)
            {
                // Evaluate branch condition, jump to next branch if false
                branch.Key.Generate(emitter);

                var next = emitter.DeclareLabel();

                emitter.InvokeValueAsBoolean();
                emitter.BranchIfFalse(next);

                // Execute branch body and jump over next branches
                if (branch.Value.Generate(emitter))
                {
                    emitter.LoadDuplicate();
                    emitter.BranchIfTrue(exitReturn);
                    emitter.Discard();

                    mayReturn = true;
                }

                emitter.BranchAlways(exitRegular);
                emitter.MarkLabel(next);
            }

            // Emit fallback branch if any or null value otherwise
            if (_fallback != null && _fallback.Generate(emitter))
            {
                emitter.LoadDuplicate();
                emitter.BranchIfTrue(exitReturn);
                emitter.Discard();

                mayReturn = true;
            }

            // End of branch
            emitter.MarkLabel(exitRegular);

            if (mayReturn)
                emitter.LoadBoolean(false);

            // Exit command
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}