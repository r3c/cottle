using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   WhileNode : Node
    {
        #region Attributes

        private INode       body;

        private IExpression test;

        #endregion

        #region Constructors

        public  WhileNode (IExpression test, INode body)
        {
            this.body = body;
            this.test = test;
        }

        #endregion

        #region Methods

        public override void    Debug (DebugWriter writer)
        {
            writer.Write (string.Format ("{{while {0}:", this.test));
            writer.Increase ();

            this.body.Debug (writer);

            writer.Decrease ();
            writer.Write ("}");
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            while (this.test.Evaluate (scope).AsBoolean)
                this.body.Print (scope, writer);
        }

        #endregion
    }
}
