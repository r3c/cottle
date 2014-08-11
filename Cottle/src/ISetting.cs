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

		char	Escape
		{
			get;
		}

		bool	Optimize
		{
			get;
		}

		Trimmer	Trimmer
		{
			get;
		}
		
		#endregion
	}
}
