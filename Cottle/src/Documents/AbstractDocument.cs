using System;
using System.Globalization;
using System.IO;

namespace Cottle.Documents
{
	public abstract class AbstractDocument : IDocument
	{
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
