using System.Text;

namespace	Cottle.Cleaners
{
	public class	NullCleaner : ICleaner
	{
		#region Methods
		
		public void GetRange (string text, out int start, out int length)
		{
			length = text.Length;
			start = 0;
		}
		
		#endregion
	}
}
