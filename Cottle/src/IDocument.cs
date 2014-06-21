using System;
using System.IO;

namespace Cottle
{
	public interface IDocument
	{
		#region Methods

		Value	Render (IStore store, TextWriter writer);

		string	Render (IStore store);

		#endregion

		#region Obsoletes

		[Obsolete ("Replace 'scope' argument by a Cottle.IStore instance")]
		Value	Render (IScope scope, TextWriter writer);

		[Obsolete ("Replace 'scope' argument by a Cottle.IStore instance")]
		string	Render (IScope scope);

		[Obsolete ("Only SimpleDocument implements 'Source' methods")]
		void	Source (TextWriter writer);

		[Obsolete ("Only SimpleDocument implements 'Source' methods")]
		string	Source ();

		#endregion
	}
}
