using System;
using System.IO;

using Cottle.Documents;
using Cottle.Settings;

namespace	Cottle
{
	[Obsolete ("Use SimpleDocument")]
	public sealed class	Document : IDocument
	{
		#region Attributes

		private readonly SimpleDocument	document;

		#endregion

		#region Constructors

		public	Document (TextReader reader, ISetting setting)
		{
			this.document = new SimpleDocument (reader, setting);
		}

		public	Document (TextReader reader)
		{
			this.document = new SimpleDocument (reader);
		}

		public	Document (string template, ISetting setting)
		{
			this.document = new SimpleDocument (template, setting);
		}

		public	Document (string template)
		{
			this.document = new SimpleDocument (template);
		}

		[Obsolete ("Please replace LexerConfig by a CustomSetting instance")]
		public	Document (TextReader reader, LexerConfig config) :
			this (reader, (ISetting)config)
		{
		}

		[Obsolete ("Please replace LexerConfig by a CustomSetting instance")]
		public	Document (string template, LexerConfig config) :
			this (template, (ISetting)config)
		{
		}

		#endregion

		#region Methods

		[Obsolete ("Please use Source(TextWriter writer) method")]
		public void	Export (TextWriter writer)
		{
			this.Source (writer);
		}

		[Obsolete ("Please use Source() method")]
		public string	Export ()
		{
			return this.Source ();
		}

		public Value Render (IScope scope, TextWriter writer)
		{
			return this.document.Render (scope, writer);
		}

		public string Render (IScope scope)
		{
			return this.document.Render (scope);
		}

		public void Source (TextWriter writer)
		{
			this.document.Source (writer);
		}

		public string Source()
		{
			return this.document.Source ();
		}

		#endregion
	}
}
