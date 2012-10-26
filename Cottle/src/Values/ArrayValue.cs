using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values
{
    public sealed class ArrayValue : Value
    {
        #region Properties

        public override bool            AsBoolean
        {
            get
            {
                return this.fields.Count > 0;
            }
        }

        public override IFunction       AsFunction
        {
            get
            {
                return null;
            }
        }

        public override decimal         AsNumber
        {
            get
            {
                return this.fields.Count;
            }
        }

        public override string          AsString
        {
            get
            {
                return string.Empty;
            }
        }

        public override FieldMap        Fields
        {
            get
            {
                return this.fields;
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

        private FieldMap    fields;

        #endregion

        #region Constructors

        public  ArrayValue (IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            this.fields = new FieldMap (pairs);
        }

        public  ArrayValue (IEnumerable<Value> values)
        {
            this.fields = new FieldMap (values);
        }

        public  ArrayValue ()
        {
            this.fields = FieldMap.Empty;
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            int                                     compare;
            IEnumerator<KeyValuePair<Value, Value>> lhs;
            IEnumerator<KeyValuePair<Value, Value>> rhs;

            if (other == null)
                return 1;

            if (this.fields.Count < other.Fields.Count)
                return -1;
            else if (this.fields.Count > other.Fields.Count)
                return 1;

            lhs = this.fields.GetEnumerator ();
            rhs = this.fields.GetEnumerator ();

            while (lhs.MoveNext () && rhs.MoveNext ())
            {
                compare = lhs.Current.Value.CompareTo (rhs.Current.Value);

                if (compare != 0)
                    return compare;
            }

            return 0;
        }

        public override int GetHashCode ()
        {
            int hash = 0;

            foreach (KeyValuePair<Value, Value> item in this.fields)
                hash = (hash << 1) ^ item.Key.GetHashCode () ^ item.Value.GetHashCode ();

            return hash;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            separator = false;

            builder.Append ('[');

            foreach (KeyValuePair<Value, Value> pair in this.fields)
            {
                if (separator)
                    builder.Append (", ");
                else
                    separator = true;

                builder.Append (pair.Key);
                builder.Append (": ");
                builder.Append (pair.Value);
            }

            builder.Append (']');

            return builder.ToString ();
        }

        #endregion
    }
}
