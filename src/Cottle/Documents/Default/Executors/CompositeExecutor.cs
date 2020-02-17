using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
{
    internal class CompositeExecutor : IExecutor
    {
        private readonly IExecutor[] _nodes;

        public CompositeExecutor(IEnumerable<IExecutor> nodes)
        {
            _nodes = nodes.ToArray();
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            foreach (var node in _nodes)
                if (node.Execute(frame, output, out result))
                    return true;

            result = VoidValue.Instance;

            return false;
        }
    }
}