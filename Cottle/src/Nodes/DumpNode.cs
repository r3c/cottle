using System;
using System.IO;

using Cottle.Values;

namespace Cottle.Nodes
{
	sealed class DumpNode : INode
	{
		#region Attributes

		private readonly IExpression expression;

		#endregion

		#region Constructors

		public	DumpNode (IExpression expression)
		{
			this.expression = expression;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			output.Write (this.expression.Evaluate (scope, output));

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
