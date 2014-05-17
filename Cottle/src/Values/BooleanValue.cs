
namespace Cottle.Values
{
	public sealed class BooleanValue : ScalarValue<bool>
	{
		#region Constants

		public static readonly BooleanValue False = new BooleanValue (false);

		public static readonly BooleanValue True = new BooleanValue (true);

		#endregion

		#region Properties

		public override bool			AsBoolean
		{
			get
			{
				return this.value;
			}
		}

		public override decimal			AsNumber
		{
			get
			{
				return this.value ? 1 : 0;
			}
		}

		public override string			AsString
		{
			get
			{
				return this.value ? "true" : string.Empty;
			}
		}

		public override ValueContent	Type
		{
			get
			{
				return ValueContent.Boolean;
			}
		}

		#endregion

		#region Constructors

		public	BooleanValue (bool value) :
			base (value, (source) => source.AsBoolean)
		{
		}

		#endregion

		#region Methods

		public override string	ToString ()
		{
			return this.value ? "<true>" : "<false>";
		}

		#endregion
	}
}
