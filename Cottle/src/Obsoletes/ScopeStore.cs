using Cottle.Stores;

namespace Cottle.Obsolete
{
	class ScopeStore : AbstractStore
	{
		private readonly IScope scope;

		public ScopeStore (IScope scope)
		{
			this.scope = scope;
		}

		public override void Enter ()
		{
			this.scope.Enter ();
		}
	
		public override bool Leave ()
		{
			return this.scope.Leave ();
		}
	
		public override void Set (Value symbol, Value value, StoreMode mode)
		{
			this.scope.Set (symbol, value, (ScopeMode)mode);
		}

		public override bool TryGet (Value symbol, out Value value)
		{
			return this.scope.Get (symbol, out value);
		}
	}
}