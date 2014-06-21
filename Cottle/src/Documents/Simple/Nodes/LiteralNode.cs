using System;
using System.IO;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class LiteralNode : INode
	{
		#region Attributes

		private readonly string	text;
		
		#endregion

		#region Constructors

		public LiteralNode (string text)
		{
			this.text = text;
		}

		#endregion

		#region Methods

		public bool Render (IStore store, TextWriter output, out Value result)
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
