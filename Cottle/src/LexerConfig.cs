using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    public class    LexerConfig
    {
        #region Properties

        public string   BlockBegin
        {
            get
            {
                return this.blockBegin;
            }
            set
            {
                this.blockBegin = value;
            }
        }

        public string   BlockContinue
        {
            get
            {
                return this.blockContinue;
            }
            set
            {
                this.blockContinue = value;
            }
        }

        public string   BlockEnd
        {
            get
            {
                return this.blockEnd;
            }
            set
            {
                this.blockEnd = value;
            }
        }

        #endregion

        #region Attributes

        private string  blockBegin = "{";

        private string  blockContinue = "|";

        private string  blockEnd = "}";

        #endregion
    }
}
