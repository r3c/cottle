using System.Text;

namespace Cottle.Cleaners
{
	public class BlankCharactersCleaner : ICleaner
	{
		#region Methods
		
		public void GetRange (string text, out int start, out int length)
		{
			int from;
			int i;
			int to;

			// Skip all leading blank characters
			for (i = 0; i < text.Length && text[i] <= ' '; )
				++i;

			from = i;

			// Skip all trailing blank characters
			for (i = text.Length - 1; i >= 0 && text[i] <= ' '; )
				--i;

			to = i + 1;

			// Select inner content if any, empty string else
			if (from < to)
			{
				length = to - from;
				start = from;
			}
			else
			{
				length = 0;
				start = 0;
			}
		}
		
		#endregion
	}
}
