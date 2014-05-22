using System;
using Cottle.Parsers.Default;

namespace Cottle.Exceptions
{
	[Obsolete ("Not thrown anymore, catch a Cottle.Exceptions.ParseException instance")]
	public class UnexpectedException : DocumentException
	{
		#region Properties

		public string	Expected
		{
			get
			{
				return string.Empty;
			}
		}

		#endregion

		#region Constructors

		internal UnexpectedException (Lexer lexer, string expected) :
			base (lexer, expected)
		{
		}

		#endregion
	}
}
