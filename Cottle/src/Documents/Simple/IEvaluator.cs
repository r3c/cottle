using System;
using System.IO;

namespace Cottle.Documents.Simple
{
	interface IEvaluator
	{
		#region Methods

		Value	Evaluate (IStore store, TextWriter output);

		string	ToString ();

		#endregion
	}
}
