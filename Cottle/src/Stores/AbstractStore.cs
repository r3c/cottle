using System;
using Cottle.Values;

namespace Cottle.Stores
{
	public abstract class AbstractStore : IStore
	{
		#region Properties

		public Value this[Value symbol]
		{
			get
			{
				Value value;

				if (this.TryGet (symbol, out value))
					return value;

				return VoidValue.Instance;
			}
			set
			{
				this.Set (symbol, value, StoreMode.Global);
			}
		}

		#endregion

		#region Methods
		
		public abstract void Enter ();
	
		public abstract bool Leave ();

		public abstract void Set (Value symbol, Value value, StoreMode mode);

		public abstract bool TryGet (Value symbol, out Value value);

		#endregion
	}
}
