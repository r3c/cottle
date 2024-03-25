using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors.Assign
{
    internal class ValueAssignStatementExecutor : AssignStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public ValueAssignStatementExecutor(Symbol symbol, StoreMode mode, IExpressionExecutor expression) :
            base(symbol, mode)
        {
            _expression = expression;
        }

        protected override Value EvaluateOperand(Runtime runtime, Frame frame, TextWriter output)
        {
            return _expression.Execute(runtime, frame, output);
        }
    }
}