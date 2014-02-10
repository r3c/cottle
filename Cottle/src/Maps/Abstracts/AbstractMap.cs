using System;
using System.Collections;
using System.Collections.Generic;

namespace Cottle.Maps.Abstracts
{
	abstract class AbstractMap : IMap
	{
		public abstract int Count
		{
			get;
		}
		
		#region Methods / Abstract
		
		public abstract bool	Contains (Value key);

		public abstract IEnumerator<KeyValuePair<Value, Value>>	GetEnumerator ();
		
		public abstract bool	TryGet (Value key, out Value value);

		#endregion
		
		#region Methods / Public

		public int	CompareTo (IMap other)
		{
			int										compare;
			IEnumerator<KeyValuePair<Value, Value>>	lhs;
			IEnumerator<KeyValuePair<Value, Value>>	rhs;

			if (other == null)
				return 1;

			if (this.Count < other.Count)
				return -1;
			else if (this.Count > other.Count)
				return 1;

			lhs = this.GetEnumerator ();
			rhs = this.GetEnumerator ();

			while (lhs.MoveNext () && rhs.MoveNext ())
			{
				compare = lhs.Current.Value.CompareTo (rhs.Current.Value);

				if (compare != 0)
					return compare;
			}

			return 0;
		}
		
		public bool	Equals (IMap other)
		{
			IEnumerator<KeyValuePair<Value, Value>> lhs;
			IEnumerator<KeyValuePair<Value, Value>> rhs;

			if (this.Count != other.Count)
				return false;

			lhs = this.GetEnumerator ();
			rhs = this.GetEnumerator ();

			while (lhs.MoveNext () && rhs.MoveNext ())
			{
				if (!lhs.Current.Key.Equals (rhs.Current.Key) || !lhs.Current.Value.Equals (rhs.Current.Value))
					return false;
			}

			return true;
		}
		
		public override bool	Equals (object obj)
		{
			IMap	other = obj as AbstractMap;

			return other != null && this.Equals (other);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}

		public override int GetHashCode ()
		{
			int hash = 0;

			foreach (KeyValuePair<Value, Value> item in this)
				hash = (hash << 1) ^ item.Key.GetHashCode () ^ item.Value.GetHashCode ();

			return hash;
		}

		#endregion
	}
}
