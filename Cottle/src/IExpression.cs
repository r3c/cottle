using System;
using System.IO;

namespace Cottle
{
	interface IExpression
	{
		#region Methods

		Value	Evaluate (IScope scope, TextWriter output);

		string	ToString ();

		#endregion
	}
}
