using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	AssignValueNode : INode
	{
		#region Attributes

		private IExpression		expression;

		private ScopeMode		mode;

		private NameExpression	name;

		#endregion

		#region Constructors

		public	AssignValueNode (NameExpression name, IExpression expression, ScopeMode mode)
		{
			this.expression = expression;
			this.mode = mode;
			this.name = name;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			this.name.Set (scope, this.expression.Evaluate (scope, output), this.mode);

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
