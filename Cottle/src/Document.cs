using System;
using System.Globalization;
using System.IO;

using Cottle.Settings;

namespace   Cottle
{
    public class    Document
    {
        #region Attributes

        private INode       root;

        private ISetting    setting;

        #endregion

        #region Constructors

        public  Document (TextReader reader, ISetting setting)
        {
            Parser  parser;

            parser = new Parser (setting);

            this.root = parser.Parse (reader);
            this.setting = setting;
        }

        public  Document (TextReader reader) :
            this (reader, DefaultSetting.Instance)
        {
        }

        public  Document (string template, ISetting setting) :
            this (new StringReader (template), setting)
        {
        }

        public  Document (string template) :
            this (new StringReader (template), DefaultSetting.Instance)
        {
        }

        [Obsolete("Please replace LexerConfig by a CustomSetting instance")]
        public  Document (TextReader reader, LexerConfig config) :
            this (reader, (ISetting)config)
        {
        }

        [Obsolete("Please replace LexerConfig by a CustomSetting instance")]
        public  Document (string template, LexerConfig config) :
            this (template, (ISetting)config)
        {
        }

        #endregion

        #region Methods

        public Value    Render (Scope scope, TextWriter writer)
        {
            Value   result;

            this.root.Render (scope, writer, out result);

            return result;
        }

        public string   Render (Scope scope)
        {
            StringWriter    writer;

            writer = new StringWriter (CultureInfo.InvariantCulture);

            this.Render (scope, writer);

            return writer.ToString ();
        }

        public void     Source (TextWriter writer)
        {
            this.root.Source (this.setting, writer);
        }

        public string   Source ()
        {
            StringWriter    writer;

            writer = new StringWriter (CultureInfo.InvariantCulture);

            this.Source (writer);

            return writer.ToString ();
        }

        [Obsolete("Please use Source(TextWriter writer) method")]
        public void     Export (TextWriter writer)
        {
            this.Source (writer);
        }

        [Obsolete("Please use Source() method")]
        public string   Export ()
        {
            return this.Source ();
        }

        #endregion
    }
}
