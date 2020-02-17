using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
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

            result = VoidValue.Instance;

            return false;
        }
    }
}