using System.Globalization;
using System.Text;

namespace Cottle.Values
{
	public sealed class StringValue : ScalarValue<string>
	{
		#region Properties

		public override bool AsBoolean
		{
			get
			{
				return !string.IsNullOrEmpty (this.value);
			}
		}

		public override decimal AsNumber
		{
			get
			{
				decimal number;

				return decimal.TryParse (this.value, NumberStyles.Number, CultureInfo.InvariantCulture, out number) ? number : 0;
			}
		}

		public override string AsString
		{
			get
			{
				return this.value;
			}
		}

		public override ValueContent Type
		{
			get
			{
				return ValueContent.String;
			}
		}

		#endregion

		#region Constructors

		public StringValue (string value) :
			base (value, (source) => source.AsString)
		{
		}

		public StringValue (char value) :
			this (value.ToString ())
		{
		}

		#endregion

		#region Methods

		public override string ToString ()
		{
			StringBuilder builder;

			builder = new StringBuilder ();
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
