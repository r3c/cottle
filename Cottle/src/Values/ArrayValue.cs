using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class ArrayValue : Value
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return this.array.Length > 0;
            }
        }

        public override IFunction   AsFunction
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
                return this.array.Length;
            }
        }

        public override string      AsString
        {
            get
            {
                return "<array>";
            }
        }

        public override KeyValuePair<Value, Value>[]    Fields
        {
            get
            {
                return this.array;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.Array;
            }
        }

        #endregion

        #region Attributes

        private KeyValuePair<Value, Value>[]    array;

        private Dictionary<Value, Value>        map;

        #endregion

        #region Constructors

        public  ArrayValue (IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            this.array = new List<KeyValuePair<Value, Value>> (pairs).ToArray ();
            this.map = new Dictionary<Value, Value> ();

            foreach (KeyValuePair<Value, Value> pair in pairs)
                this.map[pair.Key] = pair.Value;
        }

        public  ArrayValue (IEnumerable<Value> values)
        {
            Value                               key;
            List<KeyValuePair<Value, Value>>    list;
            int                                 i = 0;

            this.map = new Dictionary<Value, Value> ();

            list = new List<KeyValuePair<Value,Value>> ();

            foreach (Value value in values)
            {
                key = new NumberValue (i++);

                list.Add (new KeyValuePair<Value, Value> (key, value));

                this.map.Add (key, value);
            }

            this.array = list.ToArray ();
        }

        public  ArrayValue ()
        {
            this.array = new KeyValuePair<Value,Value>[0];
            this.map = new Dictionary<Value, Value> ();
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            int                             compare;
            KeyValuePair<Value, Value>[]    fields;
            int                             i;

            if (other == null)
                return 1;

            fields = other.Fields;

            if (this.array.Length < fields.Length)
                return -1;
            else if (this.array.Length > fields.Length)
                return 1;

            for (i = this.array.Length; i-- > 0; )
            {
                compare = this.array[i].Value.CompareTo (fields[i].Value);

                if (compare != 0)
                    return compare;
            }

            return 0;
        }

        public override bool    Find (Value key, out Value value)
        {
            return this.map.TryGetValue (key, out value);
        }

        public override int GetHashCode ()
        {
            int hash = 0;

            foreach (KeyValuePair<Value, Value> item in this.array)
                hash = (hash << 1) ^ item.Key.GetHashCode () ^ item.Value.GetHashCode ();

            return hash;
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

            foreach (KeyValuePair<Value, Value> item in this.array)
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
    }
}
