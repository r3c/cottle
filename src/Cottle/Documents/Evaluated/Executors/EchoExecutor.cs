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
            var subject = _expression.Evaluate(frame, output);
            var value = frame.Echo(subject, output);

            output.Write(value);

            result = Value.Undefined;

            return false;
        }
    }
}