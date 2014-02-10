
namespace Cottle
{
	public interface IScope
	{
		#region Properties
		
		Value	this[Value name]
		{
			get;
			set;
		}

		#endregion
		
		#region Methods

		void	Enter ();

		bool	Get (Value name, out Value value);

		bool	Leave ();

		bool	Set (Value name, Value value, ScopeMode mode);

		#endregion
	}
}
