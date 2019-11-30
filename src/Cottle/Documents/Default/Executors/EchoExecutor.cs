using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
{
    internal class EchoExecutor : IExecutor
    {
        private readonly IEvaluator _expression;

        public EchoExecutor(IEvaluator expression)
        {
            _expression = expression;
        }

        public bool Execute(Stack stack, TextWriter output, out Value result)
        {
            output.Write(_expression.Evaluate(stack, output).AsString);

            result = VoidValue.Instance;

            return false;
        }
    }
}