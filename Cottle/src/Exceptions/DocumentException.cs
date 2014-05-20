using System;

namespace Cottle.Exceptions
{
	public class DocumentException : Exception
	{
		#region Properties

		public int	Column
		{
			get
			{
				return this.column;
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

		private readonly int	column;

		private readonly int	line;

		#endregion

		#region Constructors

		public DocumentException (int column, int line, string message) :
			base (message)
		{
			this.column = column;
			this.line = line;
		}

		#endregion

		#region Obsolete

		[Obsolete ("Use Column and Line properties")]
		public int	Index
		{
			get
			{
				return 0;
			}
		}

		#endregion
	}
}
