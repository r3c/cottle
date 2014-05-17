using System.Collections.Generic;

namespace Cottle.Maps
{
	class ArrayMap : AbstractMap
	{
		#region Properties

		public override int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		#endregion

		#region Attributes

		private readonly List<KeyValuePair<Value, Value>>	array;

		#endregion

		#region Constructors

		public ArrayMap (IEnumerable<Value> array)
		{
			int key;

			this.array = new List<KeyValuePair<Value, Value>> ();

			key = 0;

			foreach (Value value in array)
				this.array.Add (new KeyValuePair<Value, Value> (key++, value));
		}

		#endregion

		#region Methods
		
		public override bool Contains (Value key)
		{
			int index;

			if (key.Type != ValueContent.Number)
				return false;

			index = (int)key.AsNumber;

			return index >= 0 && index < this.array.Count;
		}

		public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator ()
		{
			return this.array.GetEnumerator ();
		}

		public override bool TryGet (Value key, out Value value)
		{
			int index;

			if (key.Type != ValueContent.Number)
			{
				value = default (Value);

				return false;
			}

			index = (int)key.AsNumber;

			if (index < 0 || index >= this.array.Count)
			{
				value = default (Value);

				return false;
			}

			value = this.array[index].Value;

			return true;
		}

		#endregion
	}
}
