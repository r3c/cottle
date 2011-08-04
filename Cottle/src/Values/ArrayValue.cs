using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    using   ChildDictionary = Dictionary<IValue, IValue>;
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

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
                return Function.Undefined;
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

        public override ChildList   Children
        {
            get
            {
                return this.list;
            }
        }

        #endregion

        #region Attributes

        private ChildDictionary dictionary;

        private ChildList       list;

        #endregion

        #region Constructors

        public  ArrayValue (IEnumerable<KeyValuePair<IValue, IValue>> children)
        {
            this.list = new List<KeyValuePair<IValue, IValue>> (children);
            this.dictionary = new ChildDictionary (this.list.Count, ArrayValue.ValueComparer);

            foreach (KeyValuePair<IValue, IValue> pair in children)
                this.dictionary[pair.Key] = pair.Value;
        }

        public  ArrayValue ()
        {
        }

        #endregion

        #region Methods

        public override bool    Equals (IValue other)
        {
            List<KeyValuePair<IValue, IValue>>  values = other.Children;
            KeyValuePair<IValue, IValue>        x;
            KeyValuePair<IValue, IValue>        y;
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

            foreach (KeyValuePair<IValue, IValue> item in this.list)
                hash ^= item.Key.GetHashCode () ^ item.Value.GetHashCode ();

            return hash;
        }

        public override bool    Find (IValue key, out IValue value)
        {
            return this.dictionary.TryGetValue (key, out value);
        }

        public override bool    Has (IValue key)
        {
            return this.dictionary.ContainsKey (key);
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            separator = false;

            builder.Append ('[');

            foreach (KeyValuePair<IValue, IValue> item in this.list)
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

        private class   Comparer : IEqualityComparer<IValue>
	    {
            public bool Equals (IValue x, IValue y)
            {
                return x.Equals (y);
            }

            public int  GetHashCode (IValue value)
            {
                return value.GetHashCode ();
            }
        }

        #endregion
    }
}
