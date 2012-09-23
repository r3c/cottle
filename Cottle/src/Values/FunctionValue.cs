using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
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

        public override IFunction   AsFunction
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

        public override KeyValuePair<Value, Value>[]    Fields
        {
            get
            {
                return Value.emptyFields;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.Function;
            }
        }

        #endregion

        #region Attributes

        private IFunction   function;

        #endregion

        #region Constructors

        public  FunctionValue (IFunction function)
        {
            this.function = function;
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            return 0;
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
