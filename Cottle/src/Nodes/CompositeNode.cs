using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    CompositeNode : Node
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

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            IValue  result;

            foreach (INode node in this.nodes)
            {
                result = node.Apply (scope, output);

                if (result != null)
                    return result;
            }

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            foreach (INode node in this.nodes)
                node.Debug (output);
        }

        #endregion
    }
}
