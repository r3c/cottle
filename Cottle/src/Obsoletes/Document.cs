using System;
using System.IO;

using Cottle.Documents;
using Cottle.Settings;

namespace	Cottle
{
	[Obsolete ("Use SimpleDocument")]
	public sealed class	Document : SimpleDocument
	{
		#region Constructors

		public	Document (TextReader reader, ISetting setting) :
			base (reader, setting)
		{
		}

		public	Document (TextReader reader) :
			base (reader)
		{
		}

		public	Document (string template, ISetting setting) :
			base (template, setting)
		{
		}

		public	Document (string template) :
			base (template)
		{
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

		#endregion
	}
}
