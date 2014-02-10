using System;
using System.IO;

namespace Cottle.Nodes
{
	sealed class ReturnNode : INode
	{
		#region Attributes

		private readonly IExpression	expression;

		#endregion

		#region Constructors

		public	ReturnNode (IExpression expression)
		{
			this.expression = expression;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			result = this.expression.Evaluate (scope, output);

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
