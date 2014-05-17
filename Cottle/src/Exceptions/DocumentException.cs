using System;
using System.Collections.Generic;
using System.Text;

namespace Cottle.Exceptions
{
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
				return this.index;
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

		protected int	index;

		protected int	line;

		#endregion

		#region Constructors

		internal	DocumentException (Lexer lexer, string message) :
			base (message)
		{
			this.column = lexer.Column;
			this.index = lexer.Index;
			this.line = lexer.Line;
		}

		#endregion
	}
}
