using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Executors
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

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            foreach (var branch in _branches)
            {
                if (branch.Key.Evaluate(frame, output).AsBoolean)
                    return branch.Value.Execute(frame, output, out result);
            }

            if (_fallback != null)
                return _fallback.Execute(frame, output, out result);

            result = VoidValue.Instance;

            return false;
        }
    }
}