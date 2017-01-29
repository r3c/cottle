using System;

using Cottle.Maps;

namespace Cottle.Values
{
	public abstract class ScalarValue<T> : Value where
		T : IComparable<T>
	{
		#region Properties

		public override IFunction AsFunction
		{
			get
			{
				return null;
			}
		}

		public override IMap Fields
		{
			get
			{
				return EmptyMap.Instance;
			}
		}

		#endregion

		#region Attributes

		protected readonly Converter<Value, T> converter;

		protected readonly T value;

		#endregion

		#region Constructors

		protected ScalarValue (T value, Converter<Value, T> converter)
		{
			this.converter = converter;
			this.value = value;
		}

		#endregion

		#region Methods

		public override int CompareTo (Value other)
		{
			if (other == null)
				return 1;

			if (this.Type != other.Type)
				return ((int)this.Type).CompareTo ((int)other.Type);

			return this.converter (this).CompareTo (this.converter (other));
		}

		public override int GetHashCode ()
		{
			return this.value.GetHashCode ();
		}

		#endregion
	}
}