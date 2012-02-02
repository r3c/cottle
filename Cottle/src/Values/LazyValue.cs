using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace   Cottle.Values
{
    public sealed class LazyValue : Value
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return this.Resolve ().AsBoolean;
            }
        }

        public override IFunction   AsFunction
        {
            get
            {
                return this.Resolve ().AsFunction;
            }
        }

        public override decimal     AsNumber
        {
            get
            {
                return this.Resolve ().AsNumber;
            }
        }

        public override string      AsString
        {
            get
            {
                return this.Resolve ().AsString;
            }
        }

        public override KeyValuePair<Value, Value>[]    Fields
        {
            get
            {
                return this.Resolve ().Fields;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return this.Resolve ().Type;
            }
        }

        #endregion

        #region Attributes

        private Func<Value> resolver;

        private Value       value;

        #endregion

        #region Constructors

        public  LazyValue (Func<Value> resolver)
        {
            this.resolver = resolver;
            this.value = null;
        }

        #endregion

        #region Methods / Public

        public override int CompareTo (Value other)
        {
            return this.Resolve ().CompareTo (other);
        }

        public override bool    Find (Value key, out Value value)
        {
            return this.Resolve ().Find (key, out value);
        }

        public override int GetHashCode ()
        {
            return this.Resolve ().GetHashCode ();
        }

        public override bool    Has (Value key)
        {
            return this.Resolve ().Has (key);
        }

        public override string  ToString ()
        {
 	        return "<lazy>";
        }

        #endregion

        #region Methods / Private

        private Value   Resolve ()
        {
            if (this.value == null)
            {
                lock (this.resolver)
                {
                    if (this.value == null)
                        Interlocked.Exchange (ref this.value, this.resolver ());
                }
            }

            return this.value;
        }

        #endregion
    }
}
