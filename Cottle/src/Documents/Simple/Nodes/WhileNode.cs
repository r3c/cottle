using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class WhileNode : INode
	{
		#region Attributes

		private readonly INode		body;

		private readonly IEvaluator	condition;

		#endregion

		#region Constructors

		public WhileNode (IEvaluator condition, INode body)
		{
			this.body = body;
			this.condition = condition;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			while (this.condition.Evaluate (scope, output).AsBoolean)
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
			output.Write (this.condition);
			output.Write (":");

			this.body.Source (setting, output);

			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
