using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    struct  LexemMatch
    {
        #region Properties

        public string       Content
        {
            get
            {
                return this.content;
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

        private string      content;

        private LexemType   type;

        #endregion

        #region Constructors

        public  LexemMatch (LexemType type, string content)
        {
            this.content = content;
            this.type = type;
        }

        #endregion
    }
}
