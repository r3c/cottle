using System;
using System.Globalization;
using System.Windows.Forms;

using Cottle.Values;

namespace Cottle.Demo
{
	public partial class NodeForm : Form
	{
		#region Attributes

		private NodeAssignDelegate assign;

		#endregion

		#region Constructors

		public NodeForm (NodeData data, NodeAssignDelegate assign)
		{
			this.assign = assign;

			InitializeComponent ();

			if (data != null)
			{
				this.textBoxName.Text = data.Key;

				switch (data.Value.Type)
				{
					case ValueContent.Boolean:
						this.radioButtonValueBoolean.Checked = true;
						this.checkBoxValueBoolean.Checked = data.Value.AsBoolean;

						break;

					case ValueContent.Map:
						this.radioButtonValueMap.Checked = true;

						break;

					case ValueContent.Number:
						this.radioButtonValueNumber.Checked = true;
						this.textBoxValueNumber.Text = data.Value.AsNumber.ToString (CultureInfo.InvariantCulture);

						break;

					case ValueContent.String:
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

		private void buttonAccept_Click (object sender, EventArgs e)
		{
			string key = this.textBoxName.Text;
			decimal number;

			if (string.IsNullOrEmpty (key))
			{
				MessageBox.Show (this, "Please enter a non-empty name for this value.", "Invalid name");

				return;
			}

			switch (this.ApplyType ())
			{
				case ValueContent.Boolean:
					this.assign (key, this.checkBoxValueBoolean.Checked);

					break;

				case ValueContent.Map:
					this.assign (key, MapValue.Empty);

					break;

				case ValueContent.Number:
					if (!decimal.TryParse (this.textBoxValueNumber.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
					{
						MessageBox.Show (this, string.Format ("\"{0}\" is not a valid number, please enter a valid decimal value (e.g.: 5.0, 27, .897).", this.textBoxValueNumber.Text), "Invalid number");

						return;
					}

					this.assign (key, number);

					break;

				case ValueContent.String:
					this.assign (key, this.textBoxValueString.Text);

					break;

				default:
					this.assign (key, VoidValue.Instance);

					break;
			}

			this.Close ();
		}

		private void buttonCancel_Click (object sender, EventArgs e)
		{
			this.Close ();
		}

		private void radioButtonValue_CheckedChanged (object sender, EventArgs e)
		{
			this.ApplyType ();
		}

		#endregion

		#region Methods / Private

		private ValueContent ApplyType ()
		{
			ValueContent type;

			if (this.radioButtonValueBoolean.Checked)
				type = ValueContent.Boolean;
			else if (this.radioButtonValueMap.Checked)
				type = ValueContent.Map;
			else if (this.radioButtonValueNumber.Checked)
				type = ValueContent.Number;
			else if (this.radioButtonValueString.Checked)
				type = ValueContent.String;
			else
				type = ValueContent.Void;

			this.checkBoxValueBoolean.Enabled = (type == ValueContent.Boolean);
			this.textBoxValueNumber.Enabled = (type == ValueContent.Number);
			this.textBoxValueString.Enabled = (type == ValueContent.String);

			return type;
		}

		#endregion

		#region Types

		public delegate void NodeAssignDelegate (string key, Value value);

		#endregion
	}
}
