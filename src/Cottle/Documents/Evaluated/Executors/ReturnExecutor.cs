using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents.Evaluated.Executors
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