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

        private NodeAssignDelegate assign;

        #endregion

        #region Constructors

        public  NodeForm (NodeData data, NodeAssignDelegate getNode)
        {
            InitializeComponent ();

            this.assign = getNode;

            if (data != null)
            {
                this.textBoxName.Text = data.Key;

                switch (data.Value.Type)
                {
                    case Value.DataType.ARRAY:
                        this.radioButtonValueArray.Checked = true;

                        break;

                    case Value.DataType.BOOLEAN:
                        this.radioButtonValueBoolean.Checked = true;
                        this.checkBoxValueBoolean.Checked = data.Value.AsBoolean;

                        break;

                    case Value.DataType.NUMBER:
                        this.radioButtonValueNumber.Checked = true;
                        this.textBoxValueNumber.Text = data.Value.AsNumber.ToString (CultureInfo.InvariantCulture);

                        break;

                    case Value.DataType.STRING:
                        this.radioButtonValueString.Checked = true;
                        this.textBoxValueString.Text = data.Value.AsString;

                        break;

                    default:
                        this.radioButtonValueUndefined.Checked = true;

                        break;
                }
            }

            this.ApplyType ();
        }

        #endregion

        #region Methods / Listeners

        private void    buttonAccept_Click (object sender, EventArgs e)
        {
            string  key = this.textBoxName.Text;
            decimal number;

            if (string.IsNullOrEmpty (key))
            {
                MessageBox.Show (this, "Please enter a non-empty name for this value.", "Invalid name");

                return;
            }

            switch (this.ApplyType ())
            {
                case Value.DataType.ARRAY:
                    this.assign (key, new ArrayValue ());

                    break;

                case Value.DataType.BOOLEAN:
                    this.assign (key, this.checkBoxValueBoolean.Checked);

                    break;

                case Value.DataType.NUMBER:
                    if (!decimal.TryParse (this.textBoxValueNumber.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
                    {
                        MessageBox.Show (this, string.Format ("\"{0}\" is not a valid number, please enter a valid decimal value (e.g.: 5.0, 27, .897).", this.textBoxValueNumber.Text), "Invalid number");

                        return;
                    }

                    this.assign (key, number);

                    break;

                case Value.DataType.STRING:
                    this.assign (key, this.textBoxValueString.Text);

                    break;

                default:
                    this.assign (key, UndefinedValue.Instance);

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

        public delegate void    NodeAssignDelegate (string key, Value value);

        #endregion
    }
}
