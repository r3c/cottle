using System;

namespace Cottle.Parsers.Post
{
	interface IOptimizer
	{
		#region Methods

		Command		Optimize (Command command);

		Expression	Optimize (Expression expression);

		#endregion
	}
}
