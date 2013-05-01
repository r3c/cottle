using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace	Cottle
{
	interface	IExpression
	{
		#region Methods

		Value	Evaluate (IScope scope, TextWriter output);

		string	ToString ();

		#endregion
	}
}
