using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	TextNode : INode
	{
		#region Attributes

		private char[]	buffer;
		
		private int		length;
		
		private int		start;

		#endregion

		#region Constructors

		public	TextNode (string text, int start, int length)
		{
			this.buffer = text.ToCharArray ();
			this.length = length;
			this.start = start;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			output.Write (this.buffer, this.start, this.length);

			result = UndefinedValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			StringBuilder	builder;

			builder = new StringBuilder ()
				.Append (this.buffer, this.start, this.length)
				.Replace ("\\", "\\\\")
				.Replace (setting.BlockBegin, "\\" + setting.BlockBegin)
				.Replace (setting.BlockContinue, "\\" + setting.BlockContinue)
				.Replace (setting.BlockEnd, "\\" + setting.BlockEnd);

			output.Write (builder.ToString ());
		}

		#endregion
	}
}
