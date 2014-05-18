using System;
using System.Collections.Generic;
using System.IO;

using Cottle.Expressions;
using Cottle.Values;

namespace Cottle.Functions
{
	class NodeFunction : IFunction
	{
		#region Attributes

		private readonly List<NameExpression>	arguments;

		private readonly INode					body;

		#endregion

		#region Constructors

		public	NodeFunction (IEnumerable<NameExpression> arguments, INode body)
		{
			this.arguments = new List<NameExpression> (arguments);
			this.body = body;
		}

		#endregion

		#region Methods

		public Value	Execute (IList<Value> values, IScope scope, TextWriter output)
		{
			Value	result;
			int		i = 0;

			scope.Enter ();

			foreach (NameExpression argument in this.arguments)
			{
				argument.Set (scope, i < values.Count ? values[i] : VoidValue.Instance, ScopeMode.Local);

				++i;
			}

			this.body.Render (scope, output, out result);

			scope.Leave ();

			return result;
		}

		#endregion
	}
}
