
namespace Cottle
{
	public interface IStore
	{
		#region Properties
		
		Value this[Value symbol]
		{
			get;
			set;
		}

		#endregion
		
		#region Methods

		void Enter ();

		bool Leave ();

		void Set (Value symbol, Value value, StoreMode mode);

		bool TryGet (Value symbol, out Value value);

		#endregion
	}
}
