using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    public class    Document
    {
        #region Constants

        private const string    DEFAULT_INDENT = "    ";

        #endregion

        #region Properties

        public Dictionary<string, IValue>   Values
        {
            get
            {
                return this.values;
            }
        }

        #endregion

        #region Attributes

        private INode                       root;

        private Dictionary<string, IValue>  values = new Dictionary<string, IValue> ();

        #endregion

        #region Constructors

        internal    Document (INode root)
        {
            this.root = root;
        }

        #endregion

        #region Methods

        public void Debug (TextWriter writer, string indent)
        {
            this.root.Debug (new DebugWriter (writer, indent));
        }

        public void Debug (TextWriter writer)
        {
            this.Debug (writer, Document.DEFAULT_INDENT);
        }

        public string   Debug (string indent)
        {
            StringWriter    writer = new StringWriter ();

            this.Debug (writer, indent);

            return writer.ToString ();
        }

        public string   Debug ()
        {
            return this.Debug (Document.DEFAULT_INDENT);
        }

        public void Print (TextWriter writer)
        {
            Scope   scope = new Scope ();

            foreach (KeyValuePair<string, IValue> pair in this.values)
                scope.Set (pair.Key, pair.Value, Scope.SetMode.DECLARE);

            this.root.Print (scope, writer);
        }

        public string   Print ()
        {
            StringWriter    writer = new StringWriter ();

            this.Print (writer);

            return writer.ToString ();
        }

        #endregion
    }
}
