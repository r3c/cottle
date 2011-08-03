using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    using   ChildDictionary = Dictionary<string, IValue>;
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

    public abstract class   Value : IValue
    {
        #region Constants

        private static readonly ChildDictionary EmptyDictionary = new Dictionary<string, IValue> ();

        private static readonly ChildList       EmptyList = new List<KeyValuePair<IValue, IValue>> ();

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

        public ChildList            Children
        {
            get
            {
                return this.list;
            }
        }

        #endregion

        #region Attributes

        protected ChildDictionary   dictionary;

        protected ChildList         list;

        #endregion

        #region Constructors

        public  Value (IEnumerable<KeyValuePair<IValue, IValue>> children)
        {
            this.list = new List<KeyValuePair<IValue, IValue>> (children);
            this.dictionary = new ChildDictionary (this.list.Count);

            foreach (KeyValuePair<IValue, IValue> pair in children)
                this.dictionary[pair.Key.AsString] = pair.Value;
        }

        public  Value ()
        {
            this.dictionary = Value.EmptyDictionary;
            this.list = Value.EmptyList;
        }

        #endregion

        #region Methods

        public bool Find (string name, out IValue child)
        {
            return this.dictionary.TryGetValue (name, out child);
        }

        public bool Has (string name)
        {
            return this.dictionary.ContainsKey (name);
        }

        #endregion
    }
}
