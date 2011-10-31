using System;
using System.Collections.Generic;
using System.Text;

using Cottle;
using System.Windows.Forms;

namespace   Demo
{
    class   NodeData
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

        #region Methods

        public TreeNode ToNode ()
        {
            TreeNode    node;
            
            node = new TreeNode (string.Format ("{0} = {1}", this.key, this.value), this.ImageIndex, this.ImageIndex);
            node.Tag = this;

            return node;
        }

        #endregion
    }
}
