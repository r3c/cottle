using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    CompositeNode : INode
    {
        #region Attributes

        private IEnumerable<INode>  nodes;

        #endregion

        #region Constructors

        public  CompositeNode (IEnumerable<INode> nodes)
        {
            this.nodes = nodes;
        }

        #endregion

        #region Methods

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            foreach (INode node in this.nodes)
            {
                if (node.Apply (scope, output, out result))
                    return true;
            }

            result = UndefinedValue.Instance;

            return false;
        }

        public void Print (ISetting setting, TextWriter output)
        {
            foreach (INode node in this.nodes)
                node.Print (setting, output);
        }

        #endregion
    }
}
