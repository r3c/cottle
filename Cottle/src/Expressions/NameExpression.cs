using System;
using System.Collections.Generic;
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

        public IValue   Dereference (IValue value)
        {
            IValue  child;

            if (value.Find (this.name, out child))
                return child;

            return UndefinedValue.Instance;
        }

        public override IValue  Evaluate (Scope scope)
        {
            IValue  value;

            if (scope.Get (name, out value))
                return value;

            return UndefinedValue.Instance;
        }

        public bool Set (Scope scope, IValue value, Scope.SetMode mode)
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
