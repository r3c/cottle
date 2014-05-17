using System;

namespace Cottle.Exceptions
{
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
