using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   SetNode : Node
    {
        #region Atttributes

        private VarExpression   alias;

        private INode           body;

        private IExpression     value;

        #endregion

        #region Constructors

        public  SetNode (VarExpression alias, IExpression value, INode body)
        {
            this.alias = alias;
            this.body = body;
            this.value = value;
        }

        #endregion

        #region Methods

        public override void    Debug (DebugWriter writer)
        {
            writer.Write (string.Format ("{{set {0} to {1}:", this.alias, this.value));
            writer.Increase ();

            this.body.Debug (writer);

            writer.Decrease ();
            writer.Write ("}");
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            scope.Enter ();

            this.alias.Set (scope, this.value.Evaluate (scope));
            this.body.Print (scope, writer);

            scope.Leave ();
        }

        #endregion
    }
}
