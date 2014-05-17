using System;
using System.IO;

namespace Cottle
{
	interface INode
	{
		#region Methods

		bool	Render (IScope scope, TextWriter output, out Value result);

		void	Source (ISetting setting, TextWriter output);

		#endregion
	}
}
