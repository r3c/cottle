using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    using   FieldList = List<KeyValuePair<Value, Value>>;
    using   FieldMap = Dictionary<Value, Value>;

    public sealed class ArrayValue : Value
    {
        #region Constants

        private static readonly Comparer    ValueComparer = new Comparer ();

        #endregion

        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return this.list.Count > 0;
            }
        }

        public override Function    AsFunction
        {
            get
            {
                return null;
            }
        }

        public override decimal     AsNumber
        {
            get
            {
                return this.list.Count;
            }
        }

        public override string      AsString
        {
            get
            {
                return "<array>";
            }
        }

        public override FieldList   Fields
        {
            get
            {
                return this.list;
            }
        }

        #endregion

        #region Attributes

        private FieldList   list;

        private FieldMap    map;

        #endregion

        #region Constructors

        public  ArrayValue (IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            this.list = new FieldList (pairs);
            this.map = new FieldMap (this.list.Count, ArrayValue.ValueComparer);

            foreach (KeyValuePair<Value, Value> pair in pairs)
                this.map[pair.Key] = pair.Value;
        }

        public  ArrayValue (IEnumerable<Value> values)
        {
            Value   key;
            int     i = 0;

            this.list = new FieldList ();
            this.map = new FieldMap (ArrayValue.ValueComparer);

            foreach (Value value in values)
            {
                key = new NumberValue (i++);

                this.map.Add (key, value);
                this.list.Add (new KeyValuePair<Value, Value> (key, value));
            }
        }

        public  ArrayValue ()
        {
        }

        #endregion

        #region Methods

        public override bool    Equals (Value other)
        {
            List<KeyValuePair<Value, Value>>    values = other.Fields;
            KeyValuePair<Value, Value>          x;
            KeyValuePair<Value, Value>          y;
            int                                 i;

            if (this.list.Count != values.Count)
                return false;

            for (i = this.list.Count; i-- > 0; )
            {
                x = this.list[i];
                y = values[i];

                if (!x.Key.Equals (y.Key) || !x.Value.Equals (y.Key))
                    return false;
            }

            return true;
        }

        public override int GetHashCode ()
        {
            int hash = 0;

            foreach (KeyValuePair<Value, Value> item in this.list)
                hash ^= item.Key.GetHashCode () ^ item.Value.GetHashCode ();

            return hash;
        }

        public override bool    Find (Value key, out Value value)
        {
            return this.map.TryGetValue (key, out value);
        }

        public override bool    Has (Value key)
        {
            return this.map.ContainsKey (key);
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            separator = false;

            builder.Append ('[');

            foreach (KeyValuePair<Value, Value> item in this.list)
            {
                if (separator)
                    builder.Append (", ");
                else
                    separator = true;

                builder.Append (item.Key);
                builder.Append (": ");
                builder.Append (item.Value);
            }

            builder.Append (']');

            return builder.ToString ();
        }

        #endregion

        #region Types

        private class   Comparer : IEqualityComparer<Value>
	    {
            public bool Equals (Value x, Value y)
            {
                return x.Equals (y);
            }

            public int  GetHashCode (Value value)
            {
                return value.GetHashCode ();
            }
        }

        #endregion
    }
}
