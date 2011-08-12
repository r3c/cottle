using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Lexers;

namespace   Cottle.Exceptions
{
    public abstract class   LexemException : Exception
    {
        #region Properties

        public int  Column
        {
            get
            {
                return this.column;
            }
        }

        public int  Index
        {
            get
            {
                return this.index;
            }
        }

        public int  Line
        {
            get
            {
                return this.line;
            }
        }

        public abstract override string Message
        {
            get;
        }

        #endregion

        #region Attributes

        protected int   column;

        protected int   index;

        protected int   line;

        #endregion

        #region Constructors

        internal    LexemException (Lexer lexer)
        {
            this.column = lexer.Column;
            this.index = lexer.Index;
            this.line = lexer.Line;
        }

        #endregion
    }
}
