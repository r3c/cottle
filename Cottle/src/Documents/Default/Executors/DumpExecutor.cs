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

        public bool Execute(Stack stack, TextWriter output, out Value result)
        {
            output.Write(_expression.Evaluate(stack, output));

            result = VoidValue.Instance;

            return false;
        }
    }
}