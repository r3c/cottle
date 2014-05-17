using System.Collections.Generic;

namespace Cottle.Maps
{
	class HashMap : AbstractMap
	{
		#region Properties

		public override int Count
		{
			get
			{
				return this.hash.Count;
			}
		}

		#endregion

		#region Attributes

		private Dictionary<Value, Value>	hash;

		#endregion

		#region Constructors

		public HashMap (IDictionary<Value, Value> hash)
		{
			this.hash = new Dictionary<Value, Value> (hash);
		}

		#endregion

		#region Methods
		
		public override bool Contains (Value key)
		{
			return this.hash.ContainsKey (key);
		}

		public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator ()
		{
			return this.hash.GetEnumerator ();
		}

		public override bool TryGet (Value key, out Value value)
		{
			return this.hash.TryGetValue (key, out value);
		}

		#endregion
	}
}
