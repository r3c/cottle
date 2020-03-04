using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Evaluators
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

        public Value Evaluate(Frame frame, TextWriter output)
        {
            var source = _caller.Evaluate(frame, output);
            var function = source.AsFunction;

            if (function != null)
            {
                var values = new Value[_arguments.Length];

                for (var i = 0; i < _arguments.Length; ++i)
                    values[i] = _arguments[i].Evaluate(frame, output);

                return function.Invoke(frame, values, output);
            }

            return VoidValue.Instance;
        }
    }
}