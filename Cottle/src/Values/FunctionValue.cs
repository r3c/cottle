using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class FunctionValue : Value
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return false;
            }
        }

        public override Function    AsFunction
        {
            get
            {
                return this.function;
            }
        }

        public override decimal     AsNumber
        {
            get
            {
                return 0;
            }
        }

        public override string      AsString
        {
            get
            {
                return "<function>";
            }
        }

        #endregion

        #region Attributes

        private Function    function;

        #endregion

        #region Constructors

        public  FunctionValue (Function function, ICollection<KeyValuePair<IValue, IValue>> collection) :
            base (collection)
        {
            this.function = function;
        }

        public  FunctionValue (Function function)
        {
            this.function = function;
        }

        #endregion
    }
}
