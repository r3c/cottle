using System;
using System.Windows.Forms;

using Cottle;
using Cottle.Settings;

namespace   Demo
{
    public partial class    SettingForm : Form
    {
        #region Attributes

        private ConfigApplyDelegate apply;

        #endregion

        #region Constructors

        public  SettingForm (ISetting setting, ConfigApplyDelegate apply)
        {
            this.apply = apply;

            InitializeComponent ();

            foreach (SettingClean clean in Enum.GetValues (typeof (SettingClean)))
            	this.comboBoxClean.Items.Add (clean);

            this.comboBoxClean.SelectedIndex = (int)setting.Clean;
            this.textBoxBlockBegin.Text = setting.BlockBegin;
            this.textBoxBlockContinue.Text = setting.BlockContinue;
            this.textBoxBlockEnd.Text = setting.BlockEnd;
        }

        #endregion

        #region Methods

        private void    buttonAccept_Click (object sender, EventArgs e)
        {
            CustomSetting   setting;

            setting = new CustomSetting ();
            setting.BlockBegin = this.textBoxBlockBegin.Text;
            setting.BlockContinue = this.textBoxBlockContinue.Text;
            setting.BlockEnd = this.textBoxBlockEnd.Text;
            setting.Clean = (SettingClean)this.comboBoxClean.SelectedIndex;

            this.apply (setting);

            this.Close ();
        }

        private void    buttonCancel_Click (object sender, EventArgs e)
        {
            this.Close ();
        }

        #endregion

        #region Types

        public delegate void    ConfigApplyDelegate (ISetting setting);

        #endregion
    }
}
