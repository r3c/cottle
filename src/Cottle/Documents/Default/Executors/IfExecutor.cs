using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
{
    internal class IfExecutor : IExecutor
    {
        public IfExecutor(IEnumerable<KeyValuePair<IEvaluator, IExecutor>> branches, IExecutor fallback)
        {
            _branches = branches.ToArray();
            _fallback = fallback;
        }

        private readonly KeyValuePair<IEvaluator, IExecutor>[] _branches;

        private readonly IExecutor _fallback;

        public bool Execute(Stack stack, TextWriter output, out Value result)
        {
            foreach (var branch in _branches)
            {
                if (branch.Key.Evaluate(stack, output).AsBoolean)
                    return branch.Value.Execute(stack, output, out result);
            }

            if (_fallback != null)
                return _fallback.Execute(stack, output, out result);

            result = VoidValue.Instance;

            return false;
        }
    }
}