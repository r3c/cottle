using System.Globalization;

namespace Cottle.Exceptions
{
	public class UnknownException : DocumentException
	{
		#region Properties

		public string	Token
		{
			get
			{
				return this.token;
			}
		}

		#endregion

		#region Attributes

		private string	token;

		#endregion

		#region Constructors

		internal	UnknownException (Lexer lexer, string token) :
			base (lexer, string.Format (CultureInfo.InvariantCulture, "found '{0}' at line {1}, column {2}", token, lexer.Line, lexer.Column))
		{
			this.token = token;
		}

		#endregion
	}
}
