
namespace Cottle
{
	public interface IScope
	{
		#region Properties
		
		Value	this[Value symbol]
		{
			get;
			set;
		}

		#endregion
		
		#region Methods

		void	Enter ();

		bool	Get (Value symbol, out Value value);

		bool	Leave ();

		void	Set (Value symbol, Value value, ScopeMode mode);

		#endregion
	}
}
