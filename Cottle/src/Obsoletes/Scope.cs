using System;

using Cottle.Scopes;

namespace Cottle
{
	[Obsolete ("Use either Cottle.Scopes.DefaultScope or Cottle.Scopes.SimpleScope")]
	public class Scope : IScope
	{
		public Value	this[Value name]
		{
			get
			{
				return this.scope[name];
			}
			set
			{
				this.scope[name] = value;
			}
		}

		private readonly IScope	scope;

		public	Scope ()
		{
			this.scope = new SimpleScope ();
		}

		internal	Scope (IScope scope)
		{
			this.scope = scope;
		}

		public void	Enter ()
		{
			this.scope.Enter ();
		}

		public bool	Get (Value name, out Value value)
		{
			return this.scope.Get (name, out value);
		}

		public bool	Leave ()
		{
			return this.scope.Leave ();
		}

		public void	Set (Value name, Value value, ScopeMode mode)
		{
			this.scope.Set (name, value, mode);
		}
	}
}
