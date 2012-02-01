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

        public abstract int CompareTo (Value other);

        public override bool    Equals (object obj)
        {
            Value   other = obj as Value;

            return other != null && this.CompareTo (other) == 0;
        }

        public abstract bool    Find (Value key, out Value value);

        public abstract override int    GetHashCode ();

        public abstract bool    Has (Value key);

        #endregion

        #region Operators

        public static implicit operator Value (Dictionary<Value, Value> dictionary)
        {
            if (dictionary != null)
                return new ArrayValue (dictionary);

            return UndefinedValue.Instance;
        }

        public static implicit operator Value (List<KeyValuePair<Value, Value>> pairs)
        {
            if (pairs != null)
                return new ArrayValue (pairs);

            return UndefinedValue.Instance;
        }

        public static implicit operator Value (KeyValuePair<Value, Value>[] pairs)
        {
            if (pairs != null)
                return new ArrayValue (pairs);

            return UndefinedValue.Instance;
        }

        public static implicit operator Value (List<Value> list)
        {
            if (list != null)
                return new ArrayValue (list);

            return UndefinedValue.Instance;
        }

        public static implicit operator Value (Value[] array)
        {
            if (array != null)
                return new ArrayValue (array);

            return UndefinedValue.Instance;
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
            if (value != null)
                return new StringValue (value);

            return UndefinedValue.Instance;
        }

        public static implicit operator Value (char value)
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
            UNDEFINED
        }

        #endregion
    }
}
