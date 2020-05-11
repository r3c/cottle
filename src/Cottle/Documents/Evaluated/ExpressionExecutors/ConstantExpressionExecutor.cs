using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class ConstantExpressionExecutor : IExpressionExecutor
    {
        private readonly Value _value;

        public ConstantExpressionExecutor(Value value)
        {
            _value = value;
        }

        public Value Execute(Frame frame, TextWriter output)
        {
            return _value;
        }
    }
}