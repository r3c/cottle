using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class DumpNode : INode
	{
		#region Attributes

		private readonly IEvaluator expression;

		#endregion

		#region Constructors

		public DumpNode (IEvaluator expression)
		{
			this.expression = expression;
		}

		#endregion

		#region Methods

		public bool Render (IStore store, TextWriter output, out Value result)
		{
			output.Write (this.expression.Evaluate (store, output));

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			output.Write (setting.BlockBegin);
			output.Write ("dump ");
			output.Write (this.expression);
			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
