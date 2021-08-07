using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators.Assign
{
    internal class ValueAssignStatementGenerator : AssignStatementGenerator
    {
        private readonly IExpressionGenerator _expression;

        public ValueAssignStatementGenerator(Symbol symbol, StoreMode mode, IExpressionGenerator expression) :
            base(symbol, mode)
        {
            _expression = expression;
        }

        public override bool Generate(Emitter emitter)
        {
            _expression.Generate(emitter);

            GenerateAssignment(emitter);

            return false;
        }
    }
}