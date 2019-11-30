using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
{
    internal class WhileExecutor : IExecutor
    {
        public WhileExecutor(IEvaluator condition, IExecutor body)
        {
            _body = body;
            _condition = condition;
        }

        private readonly IExecutor _body;

        private readonly IEvaluator _condition;

        public bool Execute(Stack stack, TextWriter output, out Value result)
        {
            while (_condition.Evaluate(stack, output).AsBoolean)
            {
                if (_body.Execute(stack, output, out result))
                    return true;
            }

            result = VoidValue.Instance;

            return false;
        }
    }
}