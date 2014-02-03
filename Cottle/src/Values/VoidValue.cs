using Cottle.Maps;

namespace	Cottle.Values
{
	public sealed class	VoidValue : Value
	{
		#region Properties

		public override bool			AsBoolean
		{
			get
			{
				return false;
			}
		}

		public override IFunction		AsFunction
		{
			get
			{
				return null;
			}
		}

		public override decimal			AsNumber
		{
			get
			{
				return 0;
			}
		}

		public override string			AsString
		{
			get
			{
				return string.Empty;
			}
		}

		public override IMap			Fields
		{
			get
			{
				return EmptyMap.Instance;
			}
		}

		public override ValueContent	Type
		{
			get
			{
				return ValueContent.Void;
			}
		}

		#endregion

		#region Properties / Static

		public static VoidValue	Instance
		{
			get
			{
				return VoidValue.instance;
			}
		}

		#endregion

		#region Attributes

		private static readonly VoidValue	instance = new VoidValue ();

		#endregion

		#region Methods

		public override int CompareTo (Value other)
		{
			if (other == null)
				return 1;
			else if (this.Type != other.Type)
				return ((int)this.Type).CompareTo ((int)other.Type);

			return 0;
		}

		public override int GetHashCode ()
		{
			return 0;
		}

		public override string	ToString ()
		{
			return "<void>";
		}

		#endregion
	}
}
