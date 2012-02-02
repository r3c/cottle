using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace   Cottle
{
    public class    Document
    {
        #region Constants

        private const string    DEFAULT_INDENT = "    ";

        #endregion

        #region Attributes

        private INode   root;

        #endregion

        #region Constructors

        public  Document (TextReader reader)
        {
            Parser  parser = new Parser ();

            this.root = parser.Parse (reader);
        }

        public  Document (string template) :
            this(new StringReader(template))
        {
        }

        #endregion

        #region Methods

        public void Debug (TextWriter writer)
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
