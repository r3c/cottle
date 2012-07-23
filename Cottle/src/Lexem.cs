using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    class   Lexem
    {
        #region Properties

        public string       Content
        {
            get
            {
                return this.content.ToString ();
            }
        }

        public LexemType    Type
        {
            get
            {
                return this.type;
            }
        }

        #endregion

        #region Attributes

        private StringBuilder   content = new StringBuilder ();

        private LexemType       type = LexemType.None;

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
        public void Push (string value)
        {
            this.content.Append (value);
        }

        public void Push (char value)
        {
            this.content.Append (value);
        }

        public void Reset (LexemType type)
        {
            this.content.Length = 0;
            this.type = type;
        }

        #endregion
    }
}
