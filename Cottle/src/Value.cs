using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cottle.Values;

namespace   Cottle
{
    using   ChildList = List<KeyValuePair<Value, Value>>;

    public abstract class   Value
    {
        #region Constants

        protected static readonly ChildList EmptyFields = new List<KeyValuePair<Value, Value>> ();

        #endregion

        #region Properties

        public abstract bool        AsBoolean
        {
            get;
        }

        public abstract Function    AsFunction
        {
            get;
        }

        public abstract decimal     AsNumber
        {
            get;
        }

        public abstract string      AsString
        {
            get;
        }

        public abstract ChildList   Fields
        {
            get;
        }

        #endregion

        #region Methods

        public abstract bool            Equals (Value other);

        public abstract bool            Find (Value key, out Value value);

        public abstract override int    GetHashCode ();

        public abstract bool            Has (Value key);

        #endregion

        #region Operators

        public static implicit operator Value (Dictionary<Value, Value> pairs)
        {
            return new ArrayValue (pairs);
        }

        public static implicit operator Value (List<KeyValuePair<Value, Value>> pairs)
        {
            return new ArrayValue (pairs);
        }

        public static implicit operator Value (KeyValuePair<Value, Value>[] pairs)
        {
            return new ArrayValue (pairs);
        }

        public static implicit operator Value (List<Value> values)
        {
            return new ArrayValue (values);
        }

        public static implicit operator Value (Value[] values)
        {
            return new ArrayValue (values);
        }

        public static implicit operator Value (bool value)
        {
            return value ? BooleanValue.True : BooleanValue.False;
        }

        public static implicit operator Value (decimal value)
        {
            return new NumberValue (value);
        }

        public static implicit operator Value (short value)
        {
            return new NumberValue (value);
        }

        public static implicit operator Value (int value)
        {
            return new NumberValue (value);
        }

        public static implicit operator Value (long value)
        {
            return new NumberValue (value);
        }

        public static implicit operator Value (string value)
        {
            return new StringValue (value);
        }

        #endregion
    }
}
