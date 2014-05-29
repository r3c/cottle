using System;
using System.IO;

namespace Cottle
{
	public interface IDocument
	{
		#region Methods

		Value	Render (IScope scope, TextWriter writer);

		string	Render (IScope scope);

		#endregion

		#region Obsoletes

		[Obsolete ("Only SimpleDocument implements 'Source' methods")]
		void	Source (TextWriter writer);

		[Obsolete ("Only SimpleDocument implements 'Source' methods")]
		string	Source ();

		#endregion
	}
}
