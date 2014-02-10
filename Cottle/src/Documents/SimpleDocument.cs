using System;
using System.Globalization;
using System.IO;

using Cottle.Settings;

namespace Cottle.Documents
{
	public sealed class SimpleDocument : IDocument
	{
		#region Attributes

		private readonly INode		root;

		private readonly ISetting	setting;

		#endregion

		#region Constructors

		public	SimpleDocument (TextReader reader, ISetting setting)
		{
			Parser	parser;

			parser = new Parser (setting);

			this.root = parser.Parse (reader);
			this.setting = setting;
		}

		public	SimpleDocument (TextReader reader) :
			this (reader, DefaultSetting.Instance)
		{
		}

		public	SimpleDocument (string template, ISetting setting) :
			this (new StringReader (template), setting)
		{
		}

		public	SimpleDocument (string template) :
			this (new StringReader (template), DefaultSetting.Instance)
		{
		}

		#endregion

		#region Methods

		public Value	Render (IScope scope, TextWriter writer)
		{
			Value	result;

			this.root.Render (scope, writer, out result);

			return result;
		}

		public string	Render (IScope scope)
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Render (scope, writer);

			return writer.ToString ();
		}

		public void	Source (TextWriter writer)
		{
			this.root.Source (this.setting, writer);
		}

		public string	Source ()
		{
			StringWriter	writer;

			writer = new StringWriter (CultureInfo.InvariantCulture);

			this.Source (writer);

			return writer.ToString ();
		}

		#endregion
	}
}
