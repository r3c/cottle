using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class CompositeExecutor : IExecutor
    {
        private readonly IReadOnlyList<IExecutor> _nodes;

        public CompositeExecutor(IReadOnlyList<IExecutor> nodes)
        {
            _nodes = nodes;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            foreach (var node in _nodes)
            {
                if (node.Execute(frame, output, out result))
                    return true;
            }

            result = VoidValue.Instance;

            return false;
        }
    }
}