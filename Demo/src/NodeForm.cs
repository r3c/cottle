using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using Cottle;
using Cottle.Values;

namespace   Demo
{
    public partial class    NodeForm : Form
    {
        #region Attributes

        private NodeInsertDelegate  insert;

        #endregion

        #region Constructors

        public  NodeForm (NodeInsertDelegate insert)
        {
            InitializeComponent ();

            this.insert = insert;

            this.ApplyType ();
        }

        #endregion

        #region Methods / Listeners

        private void    buttonAccept_Click (object sender, EventArgs e)
        {
            string  name;
            decimal number;

            name = this.textBoxName.Text;

            if (string.IsNullOrEmpty (name))
            {
                MessageBox.Show (this, "Please enter a non-empty name for this value.", "Invalid name");

                return;
            }

            switch (this.ApplyType ())
            {
                case Value.DataType.ARRAY:
                    this.insert (new NodeData (name, new ArrayValue ()).ToNode ());

                    break;

                case Value.DataType.BOOLEAN:
                    this.insert (new NodeData (name, this.checkBoxValueBoolean.Checked).ToNode ());

                    break;

                case Value.DataType.NUMBER:
                    if (!decimal.TryParse (this.textBoxValueNumber.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
                    {
                        MessageBox.Show (this, string.Format ("\"{0}\" is not a valid number, please enter a valid decimal value (e.g.: 5.0, 27, .897).", this.textBoxValueNumber.Text), "Invalid number");

                        return;
                    }

                    this.insert (new NodeData (name, number).ToNode ());

                    break;

                case Value.DataType.STRING:
                    this.insert (new NodeData (name, this.textBoxValueString.Text).ToNode ());

                    break;

                default:
                    this.insert (new NodeData (name, UndefinedValue.Instance).ToNode ());

                    break;
            }

            this.Close ();
        }

        private void    buttonCancel_Click (object sender, EventArgs e)
        {
            this.Close ();
        }

        private void    radioButtonValue_CheckedChanged (object sender, EventArgs e)
        {
            this.ApplyType ();
        }

        #endregion

        #region Methods / Private

        private Value.DataType  ApplyType ()
        {
            Value.DataType  type;

            if (this.radioButtonValueArray.Checked)
                type = Value.DataType.ARRAY;
            else if (this.radioButtonValueBoolean.Checked)
                type = Value.DataType.BOOLEAN;
            else if (this.radioButtonValueNumber.Checked)
                type = Value.DataType.NUMBER;
            else if (this.radioButtonValueString.Checked)
                type = Value.DataType.STRING;
            else
                type = Value.DataType.UNDEFINED;

            this.checkBoxValueBoolean.Enabled = (type == Value.DataType.BOOLEAN);
            this.textBoxValueNumber.Enabled = (type == Value.DataType.NUMBER);
            this.textBoxValueString.Enabled = (type == Value.DataType.STRING);

            return type;
        }

        #endregion

        #region Types

        public delegate void    NodeInsertDelegate (TreeNode node);

        #endregion
    }
}
