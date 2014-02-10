using System;
using System.IO;
using System.Text;

using Cottle.Values;

namespace Cottle.Nodes
{
	sealed class TextNode : INode
	{
		#region Attributes

		private readonly string	text;
		
		#endregion

		#region Constructors

		public	TextNode (string text)
		{
			this.text = text;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			output.Write (this.text);

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			StringBuilder	builder;

			builder = new StringBuilder ()
				.Append (this.text)
				.Replace ("\\", "\\\\")
				.Replace (setting.BlockBegin, "\\" + setting.BlockBegin)
				.Replace (setting.BlockContinue, "\\" + setting.BlockContinue)
				.Replace (setting.BlockEnd, "\\" + setting.BlockEnd);

			output.Write (builder);
		}

		#endregion
	}
}
