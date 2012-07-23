using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Exceptions
{
    public class    UnexpectedException : LexemException
    {
        #region Properties

        public string	Current
        {
            get
            {
                return this.current;
            }
        }

        public string   Expected
        {
            get
            {
                return this.expected;
            }
        }

        public override string  Message
        {
            get
            {
                return string.Format ("Unexpected '{0}', expected {1} at line {2}, column {3}", this.current, expected, this.line, this.column);
            }
        }

        #endregion

        #region Attributes

        private string	current;

        private string	expected;

        #endregion

        #region Constructors

        internal    UnexpectedException (Lexer lexer, string expected) :
            base (lexer)
        {
            this.current = lexer.Current.Content;
            this.expected = expected;
        }

        #endregion
    }
}
