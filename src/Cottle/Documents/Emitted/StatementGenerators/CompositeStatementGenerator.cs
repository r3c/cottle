using System.Collections.Generic;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class CompositeStatementGenerator : IStatementGenerator
    {
        private readonly IReadOnlyList<IStatementGenerator> _statements;

        public CompositeStatementGenerator(IReadOnlyList<IStatementGenerator> statements)
        {
            _statements = statements;
        }

        public bool Generate(Emitter emitter)
        {
            var exitReturn = emitter.DeclareLabel();
            var mayReturn = false;

            foreach (var statement in _statements)
            {
                if (!statement.Generate(emitter))
                    continue;

                emitter.EmitLoadDuplicate();
                emitter.EmitBranchWhenTrue(exitReturn);
                emitter.EmitDiscard();

                mayReturn = true;
            }

            if (mayReturn)
                emitter.EmitLoadBoolean(false);

            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}