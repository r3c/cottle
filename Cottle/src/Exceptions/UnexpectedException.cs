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

		public string   Expected
		{
			get
			{
				return this.expected;
			}
		}

        public int  	Line
        {
            get
            {
                return this.line;
            }
        }

		public string	Value
		{
			get
			{
				return this.value;
			}
		}

        #endregion

        #region Attributes

        private int 	column;

		private string	expected;

        private int 	line;

		private string	value;

        #endregion

        #region Constructors

        internal    UnexpectedException (Lexer lexer, string expected) :
            base (string.Format ("Unexpected '{0}', expected {1} at line {2}, column {3}", lexer.Value, expected, lexer.Line, lexer.Column))
        {
            this.column = lexer.Column;
			this.expected = expected;
            this.line = lexer.Line;
			this.value = lexer.Value;
        }

        #endregion
    }
}
