using System;
using System.IO;

namespace Cottle.Documents.Simple
{
	interface INode
	{
		#region Methods

		bool Render (IStore store, TextWriter output, out Value result);

		void Source (ISetting setting, TextWriter output);

		#endregion
	}
}
