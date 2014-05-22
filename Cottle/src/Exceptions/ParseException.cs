using System;
using System.Globalization;

namespace Cottle.Exceptions
{
	public class ParseException : Exception
	{
		#region Properties

		public int Column
		{
			get
			{
				return this.column;
			}
		}

		public string Lexem
		{
			get
			{
				return this.lexem;
			}
		}

		public int Line
		{
			get
			{
				return this.line;
			}
		}

		#endregion

		#region Attributes

		private readonly int	column;

		private readonly string	lexem;

		private readonly int	line;

		#endregion

		#region Constructors

		public ParseException (int column, int line, string lexem, string expected) :
			base (string.Format (CultureInfo.InvariantCulture, !string.IsNullOrEmpty (expected) ? "expected '{1}', found '{0}'" : "unexpected '{0}'", lexem, expected))
		{
			this.column = column;
			this.lexem = lexem;
			this.line = line;
		}

		#endregion
	}
}
