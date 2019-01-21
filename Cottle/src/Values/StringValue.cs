using System.Globalization;
using System.Text;

namespace Cottle.Values
{
    public sealed class StringValue : ScalarValue<string>
    {
        #region Methods

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append('"');

            foreach (var c in Value)
            {
                if (c == '\\' || c == '"')
                    builder.Append('\\');

                builder.Append(c);
            }

            builder.Append('"');

            return builder.ToString();
        }

        #endregion

        #region Properties

        public override bool AsBoolean => !string.IsNullOrEmpty(Value);

        public override decimal AsNumber =>
            decimal.TryParse(Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number)
                ? number
                : 0;

        public override string AsString => Value;

        public override ValueContent Type => ValueContent.String;

        #endregion

        #region Constructors

        public StringValue(string value) :
            base(value, source => source.AsString)
        {
        }

        public StringValue(char value) :
            this(value.ToString())
        {
        }

        #endregion
    }
}