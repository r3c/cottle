using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Lexers;

namespace   Cottle.Exceptions
{
    public class    UnknownException : Exception
    {
        #region Properties

        public int      Column
        {
            get
            {
                return this.column;
            }
        }

        public int      Index
        {
            get
            {
                return this.index;
            }
        }

        public int      Line
        {
            get
            {
                return this.line;
            }
        }

        public string   Unknown
        {
            get
            {
                return this.unknown;
            }
        }

        #endregion

        #region Attributes

        private int     column;

        private int     index;

        private int     line;

        private string  unknown;

        #endregion

        #region Constructors

        internal    UnknownException (Lexer lexer, string unknown) :
            base (string.Format ("Found '{0}' at line {1}, column {2}", unknown, lexer.Line, lexer.Column))
        {
            this.column = lexer.Column;
            this.index = lexer.Index;
            this.line = lexer.Line;
            this.unknown = unknown;
        }

        #endregion
    }
}
