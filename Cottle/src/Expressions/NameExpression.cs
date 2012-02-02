using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{  
    sealed class    NameExpression : Expression
    {
        #region Attributes

        private string  name;

        #endregion

        #region Constructors

        public  NameExpression (string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        public override Value  Evaluate (Scope scope, TextWriter output)
        {
            Value  value;

            if (scope.Get (this.name, out value))
                return value;

            return UndefinedValue.Instance;
        }

        public bool Set (Scope scope, Value value, ScopeSet mode)
        {
            return scope.Set (this.name, value, mode);
        }

        public override string  ToString ()
        {
            return this.name;
        }

        #endregion
    }
}
