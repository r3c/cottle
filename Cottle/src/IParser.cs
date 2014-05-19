using System;
using System.IO;

namespace Cottle
{
	interface IParser
	{
		#region Methods

		Block	Parse (TextReader reader);

		#endregion
	}
}
