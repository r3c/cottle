using System;
using System.Globalization;
using System.Windows.Forms;

namespace Cottle.Demo.Forms
{
    public partial class NodeForm : Form
    {
        private readonly NodeAssignDelegate _assign;

        public NodeForm(NodeData? data, NodeAssignDelegate assign)
        {
            _assign = assign;

            InitializeComponent();

            if (data is not null)
            {
                textBoxName.Text = data.Key;

                switch (data.Value.Type)
                {
                    case ValueContent.Boolean:
                        radioButtonValueBoolean.Checked = true;
                        checkBoxValueBoolean.Checked = data.Value.AsBoolean;

                        break;

                    case ValueContent.Map:
                        radioButtonValueMap.Checked = true;

                        break;

                    case ValueContent.Number:
                        radioButtonValueNumber.Checked = true;
                        textBoxValueNumber.Text = data.Value.AsNumber.ToString(CultureInfo.InvariantCulture);

                        break;

                    case ValueContent.String:
                        radioButtonValueString.Checked = true;
                        textBoxValueString.Text = data.Value.AsString;

                        break;

                    default:
                        radioButtonValueUndefined.Checked = true;

                        break;
                }
            }

            ApplyType();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            var key = textBoxName.Text;

            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show(this, @"Please enter a non-empty name for this value.", @"Invalid name");

                return;
            }

            switch (ApplyType())
            {
                case ValueContent.Boolean:
                    _assign(key, checkBoxValueBoolean.Checked);

                    break;

                case ValueContent.Map:
                    _assign(key, Value.EmptyMap);

                    break;

                case ValueContent.Number:
                    if (!double.TryParse(textBoxValueNumber.Text, NumberStyles.Number, CultureInfo.InvariantCulture,
                        out var number))
                    {
                        MessageBox.Show(this,
                            $@"""{textBoxValueNumber.Text}"" is not a valid number, please enter a valid decimal value (e.g.: 5.0, 27, .897).",
                            @"Invalid number");

                        return;
                    }

                    _assign(key, number);

                    break;

                case ValueContent.String:
                    _assign(key, textBoxValueString.Text);

                    break;

                default:
                    _assign(key, Value.Undefined);

                    break;
            }

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButtonValue_CheckedChanged(object sender, EventArgs e)
        {
            ApplyType();
        }

        private ValueContent ApplyType()
        {
            ValueContent type;

            if (radioButtonValueBoolean.Checked)
                type = ValueContent.Boolean;
            else if (radioButtonValueMap.Checked)
                type = ValueContent.Map;
            else if (radioButtonValueNumber.Checked)
                type = ValueContent.Number;
            else if (radioButtonValueString.Checked)
                type = ValueContent.String;
            else
                type = ValueContent.Void;

            checkBoxValueBoolean.Enabled = (type == ValueContent.Boolean);
            textBoxValueNumber.Enabled = (type == ValueContent.Number);
            textBoxValueString.Enabled = (type == ValueContent.String);

            return type;
        }

        public delegate void NodeAssignDelegate(string key, Value value);
    }
}