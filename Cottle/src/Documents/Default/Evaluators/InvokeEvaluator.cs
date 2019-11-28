using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Default.Evaluators
{
    internal class InvokeEvaluator : IEvaluator
    {
        public InvokeEvaluator(IEvaluator caller, IEnumerable<IEvaluator> arguments)
        {
            _arguments = arguments.ToArray();
            _caller = caller;
        }

        private readonly IEvaluator[] _arguments;

        private readonly IEvaluator _caller;

        public Value Evaluate(Stack stack, TextWriter output)
        {
            var source = _caller.Evaluate(stack, output);
            var function = source.AsFunction;

            if (function != null)
            {
                var values = new Value[_arguments.Length];

                for (var i = 0; i < _arguments.Length; ++i)
                    values[i] = _arguments[i].Evaluate(stack, output);

                return function.Invoke(stack, values, output);
            }

            return VoidValue.Instance;
        }
    }
}