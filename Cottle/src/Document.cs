using System.Globalization;
using System.IO;

namespace   Cottle
{
    public class    Document
    {
        #region Attributes

        private INode   root;

        #endregion

        #region Constructors

        public  Document (TextReader reader, LexerConfig config)
        {
            Parser  parser;

            parser = new Parser (config);

            this.root = parser.Parse (reader);
        }

        public  Document (TextReader reader) :
            this (reader, new LexerConfig ())
        {
        }

        public  Document (string template, LexerConfig config) :
            this(new StringReader (template), config)
        {
        }

        public  Document (string template) :
            this(new StringReader (template), new LexerConfig ())
        {
        }

        #endregion

        #region Methods

        public void     Debug (TextWriter writer)
        {
            this.root.Debug (writer);
        }

        public string   Debug ()
        {
            StringWriter    writer = new StringWriter (CultureInfo.InvariantCulture);

            this.Debug (writer);

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
            StringWriter    writer = new StringWriter (CultureInfo.InvariantCulture);

            this.Render (scope, writer);

            return writer.ToString ();
        }

        #endregion
    }
}
