using System;
using System.IO;

namespace Cottle.Documents.Simple.Nodes
{
	class ReturnNode : INode
	{
		#region Attributes

		private readonly IEvaluator expression;

		#endregion

		#region Constructors

		public ReturnNode (IEvaluator expression)
		{
			this.expression = expression;
		}

		#endregion

		#region Methods

		public bool Render (IStore store, TextWriter output, out Value result)
		{
			result = this.expression.Evaluate (store, output);

			return true;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			output.Write (setting.BlockBegin);
			output.Write ("return ");
			output.Write (this.expression);
			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
