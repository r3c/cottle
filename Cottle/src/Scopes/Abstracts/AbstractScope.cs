using Cottle.Values;

namespace	Cottle.Scopes.Abstracts
{
	public abstract class	AbstractScope : IScope
	{
		#region Properties

		public Value	this[Value name]
		{
			get
			{
				Value	value;

				if (this.Get (name, out value))
					return value;

				return UndefinedValue.Instance;
			}
			set
			{
				this.Set (name, value, ScopeMode.Closest);
			}
		}

		#endregion

		#region Methods
		
		public abstract void	Enter ();
		
		public abstract bool	Get (Value name, out Value value);
		
		public abstract bool	Leave ();

		public abstract bool	Set (Value name, Value value, ScopeMode mode);

		#endregion
	}
}
