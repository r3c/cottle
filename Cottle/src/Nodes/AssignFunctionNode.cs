using System;
using System.Collections.Generic;
using System.IO;

using Cottle.Expressions;
using Cottle.Functions;
using Cottle.Values;

namespace Cottle.Nodes
{
	sealed class AssignFunctionNode : INode
	{
		#region Attributes

		private readonly List<string>	arguments;

		private readonly INode			body;

		private readonly ScopeMode		mode;

		private readonly string			name;

		#endregion

		#region Constructors

		public	AssignFunctionNode (string name, IEnumerable<string> arguments, INode body, ScopeMode mode)
		{
			this.arguments = new List<string> (arguments);
			this.body = body;
			this.mode = mode;
			this.name = name;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			scope.Set (this.name, new FunctionValue (new NodeFunction (this.arguments, this.body)), mode);

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			bool	comma;
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
			output.Write ('(');

			comma = false;

			foreach (string argument in this.arguments)
			{
				if (comma)
					output.Write (", ");
				else
					comma = true;

				output.Write (argument);
			}

			output.Write (") ");
			output.Write (link);
			output.Write (": ");

			this.body.Source (setting, output);

			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
