using System;

namespace Cottle.Parsers.Post.Optimizers
{
	abstract class AbstractOptimizer : IOptimizer
	{
		#region Methods

		public virtual Command Optimize (Command command)
		{
			return command;
		}

		public virtual Expression Optimize (Expression expression)
		{
			return expression;
		}

		#endregion
	}
}
