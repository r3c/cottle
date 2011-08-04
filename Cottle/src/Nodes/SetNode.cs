using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    SetNode : Node
    {
        #region Atttributes

        private NameExpression  name;

        private IExpression     value;

        #endregion

        #region Constructors

        public  SetNode (NameExpression name, IExpression value)
        {
            this.name = name;
            this.value = value;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            this.name.Set (scope, this.value.Evaluate (scope, output), Scope.SetMode.ANYWHERE);

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            output.Write ("{set ");
            output.Write (this.name);
            output.Write (" to ");
            output.Write (this.value);
            output.Write ('}');
        }

        #endregion
    }
}
