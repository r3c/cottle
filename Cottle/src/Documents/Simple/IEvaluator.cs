using System;
using System.IO;

namespace Cottle.Documents.Simple
{
	interface IEvaluator
	{
		#region Methods

		Value	Evaluate (IScope scope, TextWriter output);

		string	ToString ();

		#endregion
	}
}
