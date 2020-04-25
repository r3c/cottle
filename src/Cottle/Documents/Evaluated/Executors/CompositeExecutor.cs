using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;

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

            result = Value.Undefined;

            return false;
        }
    }
}