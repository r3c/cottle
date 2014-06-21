using System;
using System.Globalization;
using System.IO;
using Cottle.Obsolete;

namespace Cottle.Documents
{
	public abstract class AbstractDocument : IDocument
	{
		#region Methods / Abstract

		public abstract Value Render (IStore store, TextWriter writer);

		#endregion

		#region Methods / Public

		public string Render (IStore store)
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Render (store, writer);

			return writer.ToString ();
		}

		#endregion

		#region Obsolete

		public Value Render (IScope scope, TextWriter writer)
		{
			return this.Render (new ScopeStore (scope), writer);
		}

		public string Render (IScope scope)
		{
			return this.Render (new ScopeStore (scope));
		}

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
