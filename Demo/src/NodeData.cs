using System;
using System.Collections.Generic;
using System.Text;

using Cottle;
using System.Windows.Forms;

namespace   Demo
{
    public class    NodeData
    {
        #region Properties

        public int      ImageIndex
        {
            get
            {
                return (int)this.value.Type;
            }
        }

        public string   Key
        {
            get
            {
                return this.key;
            }
        }

        public Value    Value
        {
            get
            {
                return this.value;
            }
        }

        #endregion

        #region Attributes

        private string  key;

        private Value   value;

        #endregion

        #region Constructors

        public  NodeData (string key, Value value)
        {
            this.key = key;
            this.value = value;
        }

        #endregion
    }
}
