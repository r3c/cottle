using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class AssignFunctionNode : IFunction, INode
	{
		#region Attributes

		private readonly string[]	arguments;

		private readonly INode		body;

		private readonly ScopeMode	mode;

		private readonly string		name;

		#endregion

		#region Constructors

		public AssignFunctionNode (string name, string[] arguments, INode body, ScopeMode mode)
		{
			this.arguments = arguments;
			this.body = body;
			this.mode = mode;
			this.name = name;
		}

		#endregion

		#region Methods

		public Value Execute (IList<Value> arguments, IScope scope, TextWriter output)
		{
			Value	result;

			scope.Enter ();

			for (int i = 0; i < this.arguments.Length; ++i)
				scope.Set (this.arguments[i], i < arguments.Count ? arguments[i] : VoidValue.Instance, ScopeMode.Local);

			this.body.Render (scope, output, out result);

			scope.Leave ();

			return result;
		}

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			scope.Set (this.name, new FunctionValue (this), mode);

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
