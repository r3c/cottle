using System;
using Cottle.Parsers.Default;

namespace Cottle.Exceptions
{
	[Obsolete ("Not thrown anymore, catch a Cottle.Exceptions.ParseException instance")]
	public class UnknownException : DocumentException
	{
		#region Properties

		public string Token
		{
			get
			{
				return this.token;
			}
		}

		#endregion

		private readonly string	token;

		#region Constructors

		internal UnknownException (Lexer lexer, string token) :
			base (lexer, "unknown lexem")
		{
			this.token = token;
		}

		#endregion
	}
}
