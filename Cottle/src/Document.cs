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

        public Dictionary<string, Value>    Values
        {
            get
            {
                return this.values;
            }
        }

        #endregion

        #region Attributes

        private INode                       root;

        private Dictionary<string, Value>   values = new Dictionary<string, Value> ();

        #endregion

        #region Constructors

        internal    Document (INode root)
        {
            this.root = root;
        }

        #endregion

        #region Methods

        public void Debug (TextWriter writer)
        {
            this.root.Debug (writer);
        }

        public string   Debug ()
        {
            StringWriter    writer = new StringWriter ();

            this.Debug (writer);

            return writer.ToString ();
        }

        public Value   Print (TextWriter writer)
        {
            Value   result;
            Scope   scope = new Scope ();

            foreach (KeyValuePair<string, Value> pair in this.values)
                scope.Set (pair.Key, pair.Value, Scope.SetMode.ANYWHERE);

            this.root.Apply (scope, writer, out result);

            return result;
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
