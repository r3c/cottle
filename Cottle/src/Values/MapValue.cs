using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Maps;

namespace Cottle.Values
{
	public sealed class MapValue : Value
	{
		#region Properties / Instance

		public override bool AsBoolean
		{
			get
			{
				return this.fields.Count > 0;
			}
		}

		public override IFunction AsFunction
		{
			get
			{
				return null;
			}
		}

		public override decimal AsNumber
		{
			get
			{
				return this.fields.Count;
			}
		}

		public override string AsString
		{
			get
			{
				return string.Empty;
			}
		}

		public override IMap Fields
		{
			get
			{
				return this.fields;
			}
		}

		public override ValueContent Type
		{
			get
			{
				return ValueContent.Map;
			}
		}

		#endregion

		#region Properties / Static

		public static MapValue Empty
		{
			get
			{
				return MapValue.empty;
			}
		}

		#endregion

		#region Attributes / Instance

		private readonly IMap fields;

		#endregion

		#region Attributes / Static

		private static readonly MapValue empty = new MapValue ();

		#endregion

		#region Constructors

		public MapValue (Func<int, Value> generator, int count)
		{
			this.fields = new GeneratorMap (generator, count);
		}
		
		public MapValue (IDictionary<Value, Value> hash)
		{
			this.fields = new HashMap (hash);
		}

		public MapValue (IEnumerable<KeyValuePair<Value, Value>> pairs)
		{
			this.fields = new MixMap (pairs);
		}

		public MapValue (IEnumerable<Value> values)
		{
			this.fields = new ArrayMap (values);
		}

		public MapValue ()
		{
			this.fields = EmptyMap.Instance;
		}

		#endregion

		#region Methods

		public override int CompareTo (Value other)
		{
			if (other == null)
				return 1;

			if (this.Type != other.Type)
				return ((int)this.Type).CompareTo ((int)other.Type);

			return this.fields.CompareTo (other.Fields);
		}

		public override int GetHashCode ()
		{
			return this.fields.GetHashCode ();
		}

		public override string ToString ()
		{
			StringBuilder builder;
			bool separator;

			builder = new StringBuilder ();
			builder.Append ('[');

			separator = false;

			foreach (KeyValuePair<Value, Value> pair in this.fields)
			{
				if (separator)
					builder.Append (", ");
				else
					separator = true;

				builder.Append (pair.Key);
				builder.Append (": ");
				builder.Append (pair.Value);
			}

			builder.Append (']');

			return builder.ToString ();
		}

		#endregion
	}
}
