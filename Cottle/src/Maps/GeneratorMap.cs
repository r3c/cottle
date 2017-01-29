using System;
using System.Collections;
using System.Collections.Generic;
using Cottle.Values;

namespace Cottle.Maps
{
	class GeneratorMap : AbstractMap
	{
		#region Properties

		public override int Count
		{
			get
			{
				return this.count;
			}
		}

		#endregion

		#region Attributes

		private readonly int count;

		private readonly Func<int, Value> generator;

		#endregion

		#region Constructors

		public GeneratorMap (Func<int, Value> generator, int count)
		{
			this.count = count;
			this.generator = generator;
		}

		#endregion

		#region Methods

		public override bool Contains (Value key)
		{
			int index;

			if (key.Type != ValueContent.Number)
				return false;

			index = (int)key.AsNumber;

			return index >= 0 && index < this.count;
		}

		public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator ()
		{
			return new GeneratorEnumerator (this.generator, this.count);
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

			if (index < 0 || index >= this.count)
			{
				value = default (Value);

				return false;
			}

			value = this.generator (index);

			return true;
		}

		#endregion

		#region Types

		private class GeneratorEnumerator : IEnumerator<KeyValuePair<Value, Value>>
		{
			public KeyValuePair<Value, Value> Current
			{
				get 
				{
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			#region Attributes

			private readonly int count;

			private KeyValuePair<Value, Value> current;

			private readonly Func<int, Value> generator;

			private int index;

			#endregion

			#region Constructors

			public GeneratorEnumerator (Func<int, Value> generator, int count)
			{
				this.count = count;
				this.current = new KeyValuePair<Value, Value> (VoidValue.Instance, VoidValue.Instance);
				this.generator = generator;
				this.index = 0;
			}

			#endregion

			#region Methods

			public void Dispose ()
			{
			}

			public bool MoveNext ()
			{
				if (this.index >= this.count)
					return false;

				this.current = new KeyValuePair<Value, Value> (this.index, this.generator (this.index));

				++this.index;

				return true;
			}

			public void Reset ()
			{
				this.index = 0;
			}

			#endregion
		}

		#endregion
	}
}
