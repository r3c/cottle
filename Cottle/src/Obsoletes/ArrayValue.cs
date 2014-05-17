using System;
using System.Collections.Generic;
using System.Text;

namespace Cottle.Values
{
	[Obsolete ("Use MapValue")]
	public sealed class ArrayValue : Value
	{
		#region Properties

		public override bool			AsBoolean
		{
			get
			{
				return this.value.AsBoolean;
			}
		}

		public override IFunction		AsFunction
		{
			get
			{
				return this.value.AsFunction;
			}
		}

		public override decimal			AsNumber
		{
			get
			{
				return this.value.AsNumber;
			}
		}

		public override string			AsString
		{
			get
			{
				return this.value.AsString;
			}
		}

		public override IMap			Fields
		{
			get
			{
				return this.value.Fields;
			}
		}

		public override ValueContent	Type
		{
			get
			{
				return this.value.Type;
			}
		}

		#endregion

		#region Attributes

		private MapValue	value;

		#endregion

		#region Constructors
		
		public	ArrayValue (IDictionary<Value, Value> hash)
		{
			this.value = new MapValue (hash);
		}

		public	ArrayValue (IEnumerable<KeyValuePair<Value, Value>> pairs)
		{
			this.value = new MapValue (pairs);
		}

		public	ArrayValue (IEnumerable<Value> values)
		{
			this.value = new MapValue (values);
		}

		public	ArrayValue ()
		{
			this.value = new MapValue ();
		}

		#endregion

		#region Methods

		public override int	GetHashCode ()
		{
			return this.value.GetHashCode ();
		}
		
		public override int	CompareTo (Value other)
		{
			return this.value.CompareTo (other);
		}

		public override string	ToString()
		{
			return this.value.ToString ();
		}

		#endregion
	}
}
