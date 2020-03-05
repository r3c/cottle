using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Executors
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

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            while (_condition.Evaluate(frame, output).AsBoolean)
            {
                if (_body.Execute(frame, output, out result))
                    return true;
            }

            result = VoidValue.Instance;

            return false;
        }
    }
}