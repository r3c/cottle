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

        private NameExpression   alias;

        private IExpression     value;

        #endregion

        #region Constructors

        public  SetNode (NameExpression alias, IExpression value)
        {
            this.alias = alias;
            this.value = value;
        }

        #endregion

        #region Methods

        public override void    Debug (TextWriter writer)
        {
            writer.Write (string.Format ("{{set {0} to {1}}}", this.alias, this.value));
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            this.alias.Set (scope, this.value.Evaluate (scope), Scope.SetMode.ANYWHERE);
        }

        #endregion
    }
}
