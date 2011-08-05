using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    using   ChildList = List<KeyValuePair<Value, Value>>;

    public sealed class FunctionValue : Value
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return false;
            }
        }

        public override Function    AsFunction
        {
            get
            {
                return this.function;
            }
        }

        public override decimal     AsNumber
        {
            get
            {
                return 0;
            }
        }

        public override string      AsString
        {
            get
            {
                return "<function>";
            }
        }

        public override ChildList   Children
        {
            get
            {
                return Value.EmptyChildren;
            }
        }

        #endregion

        #region Attributes

        private Function   function;

        #endregion

        #region Constructors

        public  FunctionValue (Function function)
        {
            this.function = function;
        }

        #endregion

        #region Methods

        public override bool    Equals (Value other)
        {
            return false;
        }

        public override bool    Find (Value key, out Value value)
        {
            value = UndefinedValue.Instance;

            return false;
        }

        public override int GetHashCode ()
        {
            return this.function.GetHashCode ();
        }

        public override bool    Has (Value key)
        {
            return false;
        }

        public override string  ToString ()
        {
            return "<function>";
        }

        #endregion
    }
}
