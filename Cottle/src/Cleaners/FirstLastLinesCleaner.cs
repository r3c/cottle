using System.Text;

namespace	Cottle.Cleaners
{
	public class	FirstLastLinesCleaner : ICleaner
	{
		#region Methods
		
		public void GetRange (string text, out int start, out int length)
		{
			int from;
			int i;
			int to;

			// Skip first line if any
			for (i = 0; i < text.Length && text[i] <= ' ' && text[i] != '\n' && text[i] != '\r'; )
				++i;

			if (i >= text.Length || (text[i] != '\n' && text[i] != '\r'))
				from = 0;
			else if (i + 1 >= text.Length || text[i] == text[i + 1] || (text[i + 1] != '\n' && text[i + 1] != '\r'))
				from = i + 1;
			else
				from = i + 2;

			// Skip last line if any
			for (i = text.Length - 1; i >= 0 && text[i] <= ' ' && text[i] != '\n' && text[i] != '\r'; )
				--i;

			if (i < 0 || (text[i] != '\n' && text[i] != '\r'))
				to = text.Length;
			else if (i < 1 || text[i] == text[i - 1] || (text[i - 1] != '\n' && text[i - 1] != '\r'))
				to = i;
			else
				to = i - 1;

			// Select inner content if any, whole text else
			if (from < to)
			{
				length = to - from;
				start = from;
			}
			else
			{
				length = text.Length;
				start = 0;
			}
		}
		
		#endregion
	}
}
