using System;
using System.Collections.Generic;

using Cottle.Maps;

namespace	Cottle.Values.Generics
{
	public abstract class	ScalarValue<T> : Value
	{
		#region Properties

		public override IFunction	AsFunction
		{
			get
			{
				return null;
			}
		}

		public override IMap		Fields
		{
			get
			{
				return EmptyMap.Instance;
			}
		}

		#endregion

		#region Attributes

		protected Converter<Value, T>	converter;

		protected T						value;

		#endregion

		#region Constructors

		protected	ScalarValue (T value, Converter<Value, T> converter)
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
			else if (this.Type != other.Type)
				return ((int)this.Type).CompareTo ((int)other.Type);

			return Comparer<T>.Default.Compare (this.converter (this), this.converter (other));
		}

		public override int GetHashCode ()
		{
			return this.value.GetHashCode ();
		}

		#endregion
	}
}
