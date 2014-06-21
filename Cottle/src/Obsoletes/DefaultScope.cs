using System;

namespace Cottle.Scopes
{
	[Obsolete ("Use Cottle.Scopes.BuiltinScope")]
	public sealed class DefaultScope : AbstractScope
	{
		private readonly BuiltinScope	scope = new BuiltinScope ();

		public override void Enter ()
		{
			this.scope.Enter ();
		}

		public override bool Get (Value symbol, out Value value)
		{
			return this.scope.Get (symbol, out value);
		}

		public override bool Leave ()
		{
			return this.scope.Leave ();
		}

		public override bool Set (Value symbol, Value value, ScopeMode mode)
		{
			return this.scope.Set (symbol, value, mode);
		}
	}
}
