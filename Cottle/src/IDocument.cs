using System;
using System.IO;

namespace Cottle
{
	public interface IDocument
	{
		#region Methods

		Value Render (IStore store, TextWriter writer);

		string Render (IStore store);

		#endregion
	}
}
