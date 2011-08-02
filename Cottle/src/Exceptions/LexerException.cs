using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Exceptions
{
    class   LexerException : Exception
    {
        #region Properties

        public int  Column
        {
            get
            {
                return this.column;
            }
        }

        public int  Line
        {
            get
            {
                return this.line;
            }
        }

        #endregion

        #region Attributes

        private int column;

        private int line;

        #endregion

        #region Constructors

        public  LexerException (int line, int column, string message) :
            base (message)
        {
            this.column = column;
            this.line = line;
        }

        #endregion
    }
}
