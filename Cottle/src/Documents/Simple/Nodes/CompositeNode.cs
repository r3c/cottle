using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class CompositeNode : INode
    {
        #region Attributes

        private readonly INode[] _nodes;

        #endregion

        #region Constructors

        public CompositeNode(IEnumerable<INode> nodes)
        {
            _nodes = nodes.ToArray();
        }

        #endregion

        #region Methods

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            foreach (var node in _nodes)
                if (node.Render(store, output, out result))
                    return true;

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            foreach (var node in _nodes)
                node.Source(setting, output);
        }

        #endregion
    }
}