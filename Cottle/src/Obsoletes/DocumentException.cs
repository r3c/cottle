using System;
using Cottle.Parsers.Default;

namespace Cottle.Exceptions
{
	[Obsolete ("Not thrown anymore, catch a Cottle.Exceptions.ParseException instance")]
	public abstract class DocumentException : Exception
	{
		#region Properties

		public int	Column
		{
			get
			{
				return this.column;
			}
		}

		public int	Index
		{
			get
			{
				return 0;
			}
		}

		public int	Line
		{
			get
			{
				return this.line;
			}
		}

		#endregion

		#region Attributes

		protected int	column;

		protected int	line;

		#endregion

		#region Constructors

		internal	DocumentException (Lexer lexer, string message) :
			base (message)
		{
			this.column = lexer.Column;
			this.line = lexer.Line;
		}

		#endregion
	}
}
