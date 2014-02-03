using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	EchoNode : INode
	{
		#region Attributes

		private IExpression expression;

		#endregion

		#region Constructors

		public	EchoNode (IExpression expression)
		{
			this.expression = expression;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			output.Write (this.expression.Evaluate (scope, output).AsString);

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			string	expression;

			expression = this.expression.ToString ();

			output.Write (setting.BlockBegin);

			if (expression.StartsWith ("echo"))
				output.Write ("echo ");

			output.Write (expression);
			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
