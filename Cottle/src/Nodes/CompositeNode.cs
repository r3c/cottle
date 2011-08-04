using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   CompositeNode : Node
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

        public override void    Debug (TextWriter writer)
        {
            foreach (INode node in this.nodes)
                node.Debug (writer);
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            foreach (INode node in this.nodes)
                node.Print (scope, writer);
        }

        #endregion
    }
}
