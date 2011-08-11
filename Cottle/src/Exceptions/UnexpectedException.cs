using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Lexers;

namespace   Cottle.Exceptions
{
    public class    UnexpectedException : Exception
    {
        #region Properties

        public int  	Column
        {
            get
            {
                return this.column;
            }
        }

        public string	Data
        {
            get
            {
                return this.data;
            }
        }

        public string   Expected
        {
            get
            {
                return this.expected;
            }
        }

        public int      Index
        {
            get
            {
                return this.index;
            }
        }

        public int  	Line
        {
            get
            {
                return this.line;
            }
        }

        #endregion

        #region Attributes

        private int 	column;

        private string	data;

        private string	expected;

        private int     index;

        private int 	line;

        #endregion

        #region Constructors

        internal    UnexpectedException (Lexer lexer, string expected) :
            base (string.Format ("Unexpected '{0}', expected {1} at line {2}, column {3}", lexer.Current.Data, expected, lexer.Line, lexer.Column))
        {
            this.column = lexer.Column;
            this.data = lexer.Current.Data;
            this.expected = expected;
            this.index = lexer.Index;
            this.line = lexer.Line;
        }

        #endregion
    }
}
