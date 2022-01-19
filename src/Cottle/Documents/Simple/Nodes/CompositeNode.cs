using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cottle.Documents.Simple.Nodes
{
    internal class CompositeNode : INode
    {
        private readonly IReadOnlyList<INode> _nodes;

        public CompositeNode(IEnumerable<INode> nodes)
        {
            _nodes = nodes.ToList();
        }

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            foreach (var node in _nodes)
            {
                if (node.Render(store, output, out result))
                    return true;
            }

            result = Value.Undefined;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            foreach (var node in _nodes)
                node.Source(setting, output);
        }
    }
}