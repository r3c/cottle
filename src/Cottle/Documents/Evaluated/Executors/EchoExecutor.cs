using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class EchoExecutor : IExecutor
    {
        private readonly IEvaluator _expression;

        public EchoExecutor(IEvaluator expression)
        {
            _expression = expression;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            output.Write(_expression.Evaluate(frame, output).AsString);

            result = Value.Undefined;

            return false;
        }
    }
}