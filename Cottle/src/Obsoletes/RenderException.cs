using System;

namespace Cottle.Exceptions
{
	[Obsolete ("Not thrown anymore, connect to IDocument's Error event")]
	public class RenderException : Exception
	{
		#region Constructors

		public	RenderException (string message, Exception inner) :
			base (message, inner)
		{
		}

		#endregion
	}
}
