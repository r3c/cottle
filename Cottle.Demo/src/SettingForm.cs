using System;
using System.Windows.Forms;

namespace Cottle.Demo
{
	public partial class SettingForm : Form
	{
		#region Attributes

		private readonly ApplyCallback apply;

		#endregion

		#region Constructors

		public SettingForm (ApplyCallback apply, Parameters parameters)
		{
			this.apply = apply;

			InitializeComponent ();

			foreach (string name in TrimmerCollection.TrimmerNames)
				this.comboBoxTrimmer.Items.Add (name);

			this.comboBoxTrimmer.SelectedIndex = parameters.TrimmerIndex;
			this.textBoxBlockBegin.Text = parameters.BlockBegin;
			this.textBoxBlockContinue.Text = parameters.BlockContinue;
			this.textBoxBlockEnd.Text = parameters.BlockEnd;
		}

		#endregion

		#region Methods

		private void buttonAccept_Click (object sender, EventArgs e)
		{
			Parameters parameters;

			parameters = new Parameters
			{
				BlockBegin = this.textBoxBlockBegin.Text,
				BlockContinue = this.textBoxBlockContinue.Text,
				BlockEnd = this.textBoxBlockEnd.Text,
				TrimmerIndex = this.comboBoxTrimmer.SelectedIndex
			};

			this.apply (parameters);

			this.Close ();
		}

		private void buttonCancel_Click (object sender, EventArgs e)
		{
			this.Close ();
		}

		#endregion

		#region Types

		public delegate void ApplyCallback (Parameters config);

		public struct Parameters
		{
			public string BlockBegin;
			public string BlockContinue;
			public string BlockEnd;
			public int TrimmerIndex;
		}

		#endregion
	}
}
