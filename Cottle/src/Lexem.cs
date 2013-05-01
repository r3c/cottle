using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace	Cottle
{
	struct	Lexem
	{
		#region Properties

		public string		Content
		{
			get
			{
				return this.content;
			}
		}

		public LexemType	Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion

		#region Attributes

		private string		content;

		private LexemType	type;

		#endregion

		#region Constructors

		public	Lexem (LexemType type, string content)
		{
			this.content = content;
			this.type = type;
		}

		#endregion
	}
}
