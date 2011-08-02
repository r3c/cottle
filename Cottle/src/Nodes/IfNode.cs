using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   IfNode : Node
    {
        #region Attributes

        private INode       bodyElse;

        private INode       bodyIf;

        private IExpression test;

        #endregion

        #region Constructors

        public  IfNode (IExpression test, INode bodyIf, INode bodyElse)
        {
            this.bodyElse = bodyElse;
            this.bodyIf = bodyIf;
            this.test = test;
        }

        #endregion

        #region Methods

        public override void    Debug (DebugWriter writer)
        {
            writer.Write (string.Format ("{{if {0}:", this.test.ToString ()));
            writer.Increase ();

            this.bodyIf.Debug (writer);

            writer.Decrease ();

            if (this.bodyElse != null)
            {
                writer.Write ("|else:");
                writer.Increase ();

                this.bodyElse.Debug (writer);

                writer.Decrease ();
            }

            writer.Write ("}");
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            if (this.test.Evaluate (scope).AsBoolean)
                this.bodyIf.Print (scope, writer);
            else if (this.bodyElse != null)
                this.bodyElse.Print (scope, writer);
        }

        #endregion
    }
}
