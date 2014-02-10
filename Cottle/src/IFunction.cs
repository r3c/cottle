using System;
using System.Collections.Generic;
using System.IO;

namespace	Cottle
{
	public interface	IFunction
	{
		#region Methods

		Value	Execute (IList<Value> arguments, IScope scope, TextWriter output);

		#endregion
	}
}
