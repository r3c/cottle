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

		private readonly List<string>	arguments;

		private readonly INode			body;

		#endregion

		#region Constructors

		public	NodeFunction (IEnumerable<string> arguments, INode body)
		{
			this.arguments = new List<string> (arguments);
			this.body = body;
		}

		#endregion

		#region Methods

		public Value	Execute (IList<Value> values, IScope scope, TextWriter output)
		{
			Value	result;
			int		i = 0;

			scope.Enter ();

			foreach (string argument in this.arguments)
			{
				scope.Set (argument, i < values.Count ? values[i] : VoidValue.Instance, ScopeMode.Local);

				++i;
			}

			this.body.Render (scope, output, out result);

			scope.Leave ();

			return result;
		}

		#endregion
	}
}
