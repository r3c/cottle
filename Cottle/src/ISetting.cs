
namespace	Cottle
{
	public interface	ISetting
	{
		#region Methods
		
		string		BlockBegin
		{
			get;
		}

		string		BlockContinue
		{
			get;
		}

		string		BlockEnd
		{
			get;
		}

		ICleaner	Cleaner
		{
			get;
		}
		
		#endregion
	}
}
