using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace	Cottle.Exceptions
{
	public class	UnexpectedException : DocumentException
	{
		#region Properties

		public string	Expected
		{
			get
			{
				return this.expected;
			}
		}

		public string	Lexem
		{
			get
			{
				return this.lexem;
			}
		}

		#endregion

		#region Attributes

		private string	expected;

		private string	lexem;

		#endregion

		#region Constructors

		internal	UnexpectedException (Lexer lexer, string expected) :
			base (lexer, string.Format (CultureInfo.InvariantCulture, "unexpected '{0}', expected {1} at line {2}, column {3}", lexer.Current.Content, expected, lexer.Line, lexer.Column))
		{
			this.expected = expected;
			this.lexem = lexer.Current.Content;
		}

		#endregion
	}
}
