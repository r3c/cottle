using System;
using Cottle.Stores;

namespace Cottle.Scopes
{
	[Obsolete("Use Cottle.Stores.SimpleStore")]
	public sealed class SimpleScope : AbstractScope
	{
		private readonly IStore store = new SimpleStore ();

		public override void Enter ()
		{
			this.store.Enter ();
		}

		public override bool Get (Value symbol, out Value value)
		{
			return this.store.TryGet (symbol, out value);
		}

		public override bool Leave ()
		{
			return this.store.Leave ();
		}

		public override bool Set (Value symbol, Value value, ScopeMode mode)
		{
			this.store.Set (symbol, value, (StoreMode)mode);

			return true;
		}
	}
}
