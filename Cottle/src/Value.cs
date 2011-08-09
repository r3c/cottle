using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cottle.Values;

namespace   Cottle
{
    using   ChildList = List<KeyValuePair<Value, Value>>;

    public abstract class   Value : IComparable<Value>
    {
        #region Constants

        protected static readonly ChildList EmptyFields = new List<KeyValuePair<Value, Value>> ();

        #endregion

        #region Properties

        public abstract bool        AsBoolean
        {
            get;
        }

        public abstract IFunction   AsFunction
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

        public abstract DataType    Type
        {
            get;
        }

        #endregion

        #region Methods

        public virtual int  CompareTo (Value other)
        {
            return this.Type.CompareTo (other.Type);
        }

        public abstract bool    Find (Value key, out Value value);

        public abstract bool    Has (Value key);

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

        #region Types

        public enum DataType
        {
            ARRAY,
            BOOLEAN,
            FUNCTION,
            NUMBER,
            STRING,
            UNDEFINED,
            VOID
        }

        #endregion
    }
}
