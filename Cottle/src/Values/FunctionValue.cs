using Cottle.Maps;

namespace Cottle.Values
{
	public sealed class FunctionValue : Value
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
				return this.function;
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
				return ValueContent.Function;
			}
		}

		#endregion

		#region Attributes

		private readonly IFunction	function;

		#endregion

		#region Constructors

		public	FunctionValue (IFunction function)
		{
			this.function = function;
		}

		#endregion

		#region Methods

		public override int CompareTo (Value other)
		{
			if (other == null)
				return 1;

			return ((int)this.Type).CompareTo ((int)other.Type);
		}

		public override int GetHashCode ()
		{
			return this.function.GetHashCode ();
		}

		public override string	ToString ()
		{
			return "<" + this.function + "()>";
		}

		#endregion
	}
}
