using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cottle.Documents.Simple.Evaluators
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

        public Value Evaluate(IStore store, TextWriter output)
        {
            var source = _caller.Evaluate(store, output);
            var function = source.AsFunction;

            if (function is not null)
            {
                var values = new Value[_arguments.Length];

                for (var i = 0; i < _arguments.Length; ++i)
                    values[i] = _arguments[i].Evaluate(store, output);

                return function.Invoke(store, values, output);
            }

            return Value.Undefined;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(_caller);
            builder.Append('(');

            var comma = false;

            foreach (var argument in _arguments)
            {
                if (comma)
                    builder.Append(", ");
                else
                    comma = true;

                builder.Append(argument);
            }

            builder.Append(')');

            return builder.ToString();
        }
    }
}