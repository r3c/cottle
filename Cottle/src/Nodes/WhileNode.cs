using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	WhileNode : INode
	{
		#region Attributes

		private INode		body;

		private IExpression	test;

		#endregion

		#region Constructors

		public	WhileNode (IExpression test, INode body)
		{
			this.body = body;
			this.test = test;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			while (this.test.Evaluate (scope, output).AsBoolean)
			{
				scope.Enter ();

				if (this.body.Render (scope, output, out result))
				{
					scope.Leave ();

					return true;
				}

				scope.Leave ();
			}

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			output.Write (setting.BlockBegin);
			output.Write ("while ");
			output.Write (this.test);
			output.Write (":");

			this.body.Source (setting, output);

			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
