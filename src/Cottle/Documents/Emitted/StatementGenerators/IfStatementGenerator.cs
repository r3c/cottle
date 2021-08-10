using System.Collections.Generic;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class IfStatementGenerator : IStatementGenerator
    {
        private readonly IReadOnlyList<KeyValuePair<IExpressionGenerator, IStatementGenerator>> _branches;
        private readonly IStatementGenerator? _fallback;

        public IfStatementGenerator(IReadOnlyList<KeyValuePair<IExpressionGenerator, IStatementGenerator>> branches,
            IStatementGenerator? fallback)
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

                var condition = emitter.EmitDeclareLocalAndStore<Value>();
                var next = emitter.DeclareLabel();

                emitter.EmitLoadLocalAddressAndRelease(condition);
                emitter.EmitCallValueAsBoolean();
                emitter.EmitBranchWhenFalse(next);

                // Execute branch body and jump over next branches
                if (branch.Value.Generate(emitter))
                {
                    emitter.EmitLoadDuplicate();
                    emitter.EmitBranchWhenTrue(exitReturn);
                    emitter.EmitDiscard();

                    mayReturn = true;
                }

                emitter.EmitBranchAlways(exitRegular);
                emitter.MarkLabel(next);
            }

            // Emit fallback branch if any or null value otherwise
            if (_fallback is not null && _fallback.Generate(emitter))
            {
                emitter.EmitLoadDuplicate();
                emitter.EmitBranchWhenTrue(exitReturn);
                emitter.EmitDiscard();

                mayReturn = true;
            }

            // End of branch
            emitter.MarkLabel(exitRegular);

            if (mayReturn)
                emitter.EmitLoadBoolean(false);

            // Exit statement
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}