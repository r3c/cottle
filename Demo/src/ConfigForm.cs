using System;
using System.Windows.Forms;

using Cottle;

namespace   Demo
{
    public partial class    ConfigForm : Form
    {
        #region Attributes

        private ConfigApplyDelegate apply;

        #endregion

        #region Constructors

        public  ConfigForm (LexerConfig config, ConfigApplyDelegate apply)
        {
            this.apply = apply;

            InitializeComponent ();

            this.textBoxBlockBegin.Text = config.BlockBegin;
            this.textBoxBlockContinue.Text = config.BlockContinue;
            this.textBoxBlockEnd.Text = config.BlockEnd;
        }

        #endregion

        #region Methods / Listeners

        private void    buttonAccept_Click (object sender, EventArgs e)
        {
            LexerConfig config;

            config = new LexerConfig ();
            config.BlockBegin = this.textBoxBlockBegin.Text;
            config.BlockContinue = this.textBoxBlockContinue.Text;
            config.BlockEnd = this.textBoxBlockEnd.Text;

            this.apply (config);

            this.Close ();
        }

        private void    buttonCancel_Click (object sender, EventArgs e)
        {
            this.Close ();
        }

        #endregion

        #region Types

        public delegate void    ConfigApplyDelegate (LexerConfig config);

        #endregion
    }
}
