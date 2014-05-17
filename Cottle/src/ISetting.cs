using System;

namespace Cottle
{
	public interface ISetting
	{
		#region Methods
		
		string	BlockBegin
		{
			get;
		}

		string	BlockContinue
		{
			get;
		}

		string	BlockEnd
		{
			get;
		}

		Trimmer	Trimmer
		{
			get;
		}
		
		#endregion

		#region Obsoletes

		ICleaner	Cleaner
		{
			get;
		}

		#endregion
	}
}
