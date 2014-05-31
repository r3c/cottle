using System;
using System.Text.RegularExpressions;

namespace Cottle.Builtins
{
	public static class BuiltinTrimmers
	{
		#region Attributes / Public

		public static readonly Trimmer	CollapseBlankCharacters = (text) => BuiltinTrimmers.collapseBlankCharacters.Replace (text, " ");

		public static readonly Trimmer	FirstAndLastBlankLines = (text) =>
		{
			int index;
			int start;
			int stop;

			// Skip first line if any
			for (index = 0; index < text.Length && text[index] <= ' ' && text[index] != '\n' && text[index] != '\r'; )
				++index;

			if (index >= text.Length || (text[index] != '\n' && text[index] != '\r'))
				start = 0;
			else if (index + 1 >= text.Length || text[index] == text[index + 1] || (text[index + 1] != '\n' && text[index + 1] != '\r'))
				start = index + 1;
			else
				start = index + 2;

			// Skip last line if any
			for (index = text.Length - 1; index >= 0 && text[index] <= ' ' && text[index] != '\n' && text[index] != '\r'; )
				--index;

			if (index < 0 || (text[index] != '\n' && text[index] != '\r'))
				stop = text.Length;
			else if (index < 1 || text[index] == text[index - 1] || (text[index - 1] != '\n' && text[index - 1] != '\r'))
				stop = index;
			else
				stop = index - 1;

			// Select inner content if any, whole text else
			if (start < stop)
				return text.Substring (start, stop - start);

			return text;
		};

		public static readonly Trimmer	LeadAndTrailBlankCharacters = (text) =>
		{
			int index;
			int start;
			int stop;

			// Skip all leading blank characters
			for (index = 0; index < text.Length && text[index] <= ' '; )
				++index;

			start = index;

			// Skip all trailing blank characters
			for (index = text.Length - 1; index >= 0 && text[index] <= ' '; )
				--index;

			stop = index + 1;

			// Select inner content if any, empty string else
			if (start < stop)
				return text.Substring (start, stop - start);

			return string.Empty;
		};

		#endregion

		#region Attributes / Private

		private static readonly Regex	collapseBlankCharacters = new Regex ("\\s{2,}", RegexOptions.Multiline);

		#endregion
	}
}
