using System;
using System.Globalization;
using System.IO;

namespace Cottle.Documents
{
	public abstract class AbstractDocument : IDocument
	{
		#region Events
		
		public event DocumentError	Error;

		#endregion

		#region Methods / Abstract

		public abstract Value Render (IScope scope, TextWriter writer);

		#endregion

		#region Methods / Public

		public string Render (IScope scope)
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Render (scope, writer);

			return writer.ToString ();
		}

		#endregion

		#region Methods / Protected

		protected void OnError (Value source, string message, Exception exception)
		{
			DocumentError	error;

			error = this.Error;

			if (error != null)
				error (source, message, exception);
		}

		#endregion

		#region Obsolete

		public virtual void	Source (TextWriter writer)
		{
			throw new NotImplementedException ();
		}

		public string Source ()
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Source (writer);

			return writer.ToString ();
		}

		#endregion
	}
}
