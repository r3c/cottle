using System;
using System.Collections.Generic;

using Cottle.Maps.Abstracts;

namespace	Cottle.Maps
{
	sealed class	EmptyMap : AbstractMap
	{
		#region Properties / Instance

		public override int Count
		{
			get
			{
				return 0;
			}
		}

		#endregion

		#region Properties / Static
		
		public static EmptyMap	Instance
		{
			get
			{
				return EmptyMap.instance;
			}
		}

		#endregion

		#region Attributes
		
		private static readonly EmptyMap							instance = new EmptyMap ();

		private static readonly IList<KeyValuePair<Value, Value>>	pairs = new KeyValuePair<Value, Value>[0];
		
		#endregion
		
		#region Methods

		public override bool	Contains (Value key)
		{
			return false;
		}
		
		public override bool	TryGet (Value key, out Value value)
		{
			value = default (Value);

			return false;
		}

		public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator ()
		{
			return EmptyMap.pairs.GetEnumerator ();
		}

		public override int GetHashCode ()
		{
			return 0;
		}

		#endregion
	}
}
