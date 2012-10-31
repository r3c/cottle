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

        public void     Export (TextWriter writer)
        {
            this.root.Print (this.setting, writer);
        }

        public string   Export ()
        {
            StringWriter    writer;

            writer = new StringWriter (CultureInfo.InvariantCulture);

            this.Export (writer);

            return writer.ToString ();
        }

        public Value    Render (Scope scope, TextWriter writer)
        {
            Value   result;

            this.root.Apply (scope, writer, out result);

            return result;
        }

        public string   Render (Scope scope)
        {
            StringWriter    writer;

            writer = new StringWriter (CultureInfo.InvariantCulture);

            this.Render (scope, writer);

            return writer.ToString ();
        }

        #endregion
    }
}
