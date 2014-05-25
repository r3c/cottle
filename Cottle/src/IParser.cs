using System;
using System.IO;

namespace Cottle
{
	interface IParser
	{
		#region Methods

		Command	Parse (TextReader reader);

		#endregion
	}
}
