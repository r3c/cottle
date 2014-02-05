using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Cottle.Settings;

namespace	Cottle.Demo
{
	public partial class	SettingForm : Form
	{
		#region Attributes

		private ConfigApplyDelegate apply;

		#endregion

		#region Constructors

		public	SettingForm (ISetting setting, ConfigApplyDelegate apply)
		{
			this.apply = apply;

			InitializeComponent ();

			foreach (KeyValuePair<string, ICleaner> cleaner in CleanerCollection.Cleaners)
				this.comboBoxClean.Items.Add (cleaner.Key);

			this.comboBoxClean.SelectedIndex = CleanerCollection.GetIndex (setting.Cleaner);
			this.textBoxBlockBegin.Text = setting.BlockBegin;
			this.textBoxBlockContinue.Text = setting.BlockContinue;
			this.textBoxBlockEnd.Text = setting.BlockEnd;
		}

		#endregion

		#region Methods

		private void	buttonAccept_Click (object sender, EventArgs e)
		{
			CustomSetting	setting;

			setting = new CustomSetting ();
			setting.BlockBegin = this.textBoxBlockBegin.Text;
			setting.BlockContinue = this.textBoxBlockContinue.Text;
			setting.BlockEnd = this.textBoxBlockEnd.Text;
			setting.Cleaner = CleanerCollection.GetCleaner (this.comboBoxClean.SelectedIndex);

			this.apply (setting);

			this.Close ();
		}

		private void	buttonCancel_Click (object sender, EventArgs e)
		{
			this.Close ();
		}

		#endregion

		#region Types

		public delegate void	ConfigApplyDelegate (ISetting setting);

		#endregion
	}
}
