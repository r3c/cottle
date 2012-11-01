using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    AssignValueNode : INode
    {
        #region Attributes

        private IExpression     expression;

        private ScopeMode       mode;

        private NameExpression  name;

        #endregion

        #region Constructors

        public  AssignValueNode (NameExpression name, IExpression expression, ScopeMode mode)
        {
            this.expression = expression;
            this.mode = mode;
            this.name = name;
        }

        #endregion

        #region Methods

        public bool Render (Scope scope, TextWriter output, out Value result)
        {
            this.name.Set (scope, this.expression.Evaluate (scope, output), this.mode);

            result = UndefinedValue.Instance;

            return false;
        }

        public void Source (ISetting setting, TextWriter output)
        {
            output.Write (setting.BlockBegin);
            output.Write ("set ");
            output.Write (this.name);

            switch (this.mode)
            {
                case ScopeMode.Closest:
                    output.Write (" to ");

                    break;

                case ScopeMode.Local:
                    output.Write (" as ");

                    break;
            }

            output.Write (this.expression);
            output.Write (setting.BlockEnd);
        }

        #endregion
    }
}
