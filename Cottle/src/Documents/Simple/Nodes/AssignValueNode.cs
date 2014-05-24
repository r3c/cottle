using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class AssignValueNode : INode
	{
		#region Attributes

		private readonly IEvaluator	expression;

		private readonly ScopeMode	mode;

		private readonly string		name;

		#endregion

		#region Constructors

		public AssignValueNode (string name, IEvaluator expression, ScopeMode mode)
		{
			this.expression = expression;
			this.mode = mode;
			this.name = name;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			scope.Set (this.name, this.expression.Evaluate (scope, output), this.mode);

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			string	keyword;
			string	link;

			switch (this.mode)
			{
				case ScopeMode.Local:
					keyword = "declare";
					link = "as";

					break;

				default:
					keyword = "set";
					link = "to";

					break;
			}

			output.Write (setting.BlockBegin);
			output.Write (keyword);
			output.Write (' ');
			output.Write (this.name);
			output.Write (' ');
			output.Write (link);
			output.Write (' ');
			output.Write (this.expression);
			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
