using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    class   DebugWriter
    {
        #region Attributes

        private string      indent;

        private int         level = 0;

        private TextWriter  writer;

        #endregion

        #region Constructors

        public  DebugWriter (TextWriter writer, string indent)
        {
            this.indent = indent;
            this.writer = writer;
        }

        #endregion

        #region Methods

        public void Decrease ()
        {
            --this.level;
        }

        public void Increase ()
        {
            ++this.level;
        }

        public void Write (string line)
        {
            int i;

            for (i = this.level; i-- > 0; )
                this.writer.Write (this.indent);

            this.writer.WriteLine (line);
        }

        #endregion
    }
}
