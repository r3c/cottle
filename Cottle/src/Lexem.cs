using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    class   Lexem
    {
        #region Properties

        public string       Data
        {
            get
            {
                return this.builder.ToString ();
            }
        }

        public LexemType    Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        #endregion

        #region Attributes

        private StringBuilder   builder = new StringBuilder ();

        private LexemType       type;

        #endregion

        #region Methods
/*
        public void Clean ()
        {
            int length = this.builder.Length;

            if (length > 1 && this.builder[length - 1] <= ' ' && this.builder[length - 2] <= ' ')
            {
                for (--length; length > 1 && this.builder[length - 2] <= ' '; )
                    --length;

                this.builder.Length = length;
            }
        }
*/
        public void Flush ()
        {
            this.builder.Length = 0;
        }

        public void Push (string value)
        {
            this.builder.Append (value);
        }

        public void Push (char value)
        {
            this.builder.Append (value);
        }

        #endregion
    }
}
