using System.IO;

namespace Cottle.Documents.Default.Executors
{
    internal class ReturnExecutor : IExecutor
    {
        private readonly IEvaluator _expression;

        public ReturnExecutor(IEvaluator expression)
        {
            _expression = expression;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            result = _expression.Evaluate(frame, output);

            return true;
        }
    }
}