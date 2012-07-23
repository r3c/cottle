using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Exceptions
{
    public class    UnknownException : LexemException
    {
        #region Properties

        public string   Problem
        {
            get
            {
                return this.problem;
            }
        }

        public override string  Message
        {
            get
            {
                return string.Format ("Found '{0}' at line {1}, column {2}", this.problem, this.line, this.column);
            }
        }

        #endregion

        #region Attributes

        private string  problem;

        #endregion

        #region Constructors

        internal    UnknownException (Lexer lexer, string problem) :
            base (lexer)
        {
            this.problem = problem;
        }

        #endregion
    }
}
