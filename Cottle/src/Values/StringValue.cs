using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class StringValue : ScalarValue<string>
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return !string.IsNullOrEmpty (this.value);
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
                decimal number;

                return decimal.TryParse (this.value, NumberStyles.Number, CultureInfo.InvariantCulture, out number) ? number : 0;
            }
        }

        public override string      AsString
        {
            get
            {
                return this.value;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.String;
            }
        }

        #endregion

        #region Constructors

        public  StringValue (string value) :
            base (value)
        {
        }

        public  StringValue (char value) :
            base (value.ToString ())
        {
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            return other != null ? this.comparer.Compare (this.AsString, other.AsString) : 1;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();

            builder.Append ('"');

            foreach (char c in this.value)
            {
                if (c == '\\' || c == '"')
                    builder.Append ('\\');

                builder.Append (c);
            }

            builder.Append ('"');

            return builder.ToString ();
        }

        #endregion
    }
}
