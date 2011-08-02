using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values
{
    public sealed class DictionaryValue : IValue
    {
        #region Properties

        public bool                         AsBoolean
        {
            get
            {
                return this.dictionary.Count > 0;
            }
        }

        public decimal                      AsNumber
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        public string                       AsString
        {
            get
            {
                return this.dictionary.Count.ToString ();
            }
        }

        public IDictionary<string, IValue>  Children
        {
            get
            {
                return this.dictionary;
            }
        }

        #endregion

        #region Attributes

        private Dictionary<string, IValue>  dictionary;

        #endregion

        #region Constructors

        public  DictionaryValue (IDictionary<string, IValue> dictionary)
        {
            this.dictionary = new Dictionary<string, IValue> (dictionary);
        }

        #endregion
    }
}
