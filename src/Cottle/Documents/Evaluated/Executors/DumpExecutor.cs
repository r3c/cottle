using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class DumpExecutor : IExecutor
    {
        private readonly IEvaluator _expression;

        public DumpExecutor(IEvaluator expression)
        {
            _expression = expression;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            output.Write(_expression.Evaluate(frame, output));

            result = Value.Undefined;

            return false;
        }
    }
}