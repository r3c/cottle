using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values;

namespace   Cottle
{
    public abstract class   Value : IComparable<Value>, IEquatable<Value>
    {
        #region Properties

        public abstract bool            AsBoolean
        {
            get;
        }

        public abstract IFunction       AsFunction
        {
            get;
        }

        public abstract decimal         AsNumber
        {
            get;
        }

        public abstract string          AsString
        {
            get;
        }

        public abstract KeyValuePair<Value, Value>[]    Fields
        {
            get;
        }

        public abstract ValueContent    Type
        {
            get;
        }

        #endregion

        #region Attributes

        protected static readonly KeyValuePair<Value, Value>[]	emptyFields = new KeyValuePair<Value, Value>[0];

        #endregion

        #region Operators

        public static bool  operator == (Value lhs, Value rhs)
        {
            return object.ReferenceEquals (lhs, null) ? object.ReferenceEquals (rhs, null) : lhs.CompareTo (rhs) == 0;
        }

        public static bool  operator != (Value lhs, Value rhs)
        {
            return object.ReferenceEquals (lhs, null) ? !object.ReferenceEquals (rhs, null) : lhs.CompareTo (rhs) != 0;
        }

        public static bool  operator < (Value lhs, Value rhs)
        {
            return !object.ReferenceEquals (rhs, null) && rhs.CompareTo (lhs) > 0;
        }

        public static bool  operator > (Value lhs, Value rhs)
        {
            return !object.ReferenceEquals (lhs, null) && lhs.CompareTo (rhs) > 0;
        }

        public static implicit operator Value (Func<Value> resolver)
        {
            if (resolver != null)
                return new LazyValue (resolver);

            return UndefinedValue.Instance;
        }

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

        #region Methods

        public abstract int CompareTo (Value other);

        public virtual bool Equals(Value other)
        {
            return this.CompareTo (other) == 0;
        }

        public override bool	Equals (object obj)
        {
            Value   other = obj as Value;

            return other != null && this.CompareTo (other) == 0;
        }

        public abstract bool    Find (Value key, out Value value);

        public abstract override int    GetHashCode ();

        public abstract bool    Has (Value key);

        #endregion
    }
}
