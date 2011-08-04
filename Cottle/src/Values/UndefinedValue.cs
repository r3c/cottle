using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

    public sealed class UndefinedValue : IValue
    {
        #region Constants

        private static readonly ChildList       EmptyList = new List<KeyValuePair<IValue, IValue>> ();

        public static readonly UndefinedValue   Instance = new UndefinedValue ();

        #endregion

        #region Properties

        public bool         AsBoolean
        {
            get
            {
                return false;
            }
        }

        public Function     AsFunction
        {
            get
            {
                return Function.Undefined;
            }
        }

        public decimal      AsNumber
        {
            get
            {
                return 0;
            }
        }

        public string       AsString
        {
            get
            {
                return "<undefined>";
            }
        }

        public ChildList    Children
        {
            get
            {
                return UndefinedValue.EmptyList;
            }
        }

        #endregion

        #region Methods

        public bool Find (string name, out IValue child)
        {
            child = null;

            return false;
        }

        public bool Has (string name)
        {
            return false;
        }

        public override string  ToString ()
        {
            return "<undefined>";
        }

        #endregion
    }
}
