using System.IO;

namespace Cottle.Documents.Default.Executors.Assign
{
    internal class ValueAssignExecutor : AssignExecutor
    {
        private readonly IEvaluator _expression;

        public ValueAssignExecutor(Symbol symbol, IEvaluator expression) :
            base(symbol)
        {
            _expression = expression;
        }

        protected override Value Evaluate(Frame frame, TextWriter output)
        {
            return _expression.Evaluate(frame, output);
        }
    }
}