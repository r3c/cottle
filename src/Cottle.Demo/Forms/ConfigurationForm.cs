using System;
using System.Windows.Forms;

namespace Cottle.Demo.Forms
{
    public partial class ConfigurationForm : Form
    {
        public DocumentConfiguration Configuration { get; private set; }

        public ConfigurationForm(DocumentConfiguration configuration)
        {
            InitializeComponent();

            foreach (var name in TrimmerCollection.TrimmerNames)
                comboBoxTrimmer.Items.Add(name);

            comboBoxTrimmer.SelectedIndex = TrimmerCollection.GetTrimmerIndex(configuration.Trimmer);
            textBoxBlockBegin.Text = configuration.BlockBegin;
            textBoxBlockContinue.Text = configuration.BlockContinue;
            textBoxBlockEnd.Text = configuration.BlockEnd;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            Configuration = new DocumentConfiguration
            {
                BlockBegin = textBoxBlockBegin.Text,
                BlockContinue = textBoxBlockContinue.Text,
                BlockEnd = textBoxBlockEnd.Text,
                Trimmer = TrimmerCollection.GetTrimmerFunction(comboBoxTrimmer.SelectedIndex)
            };

            DialogResult = DialogResult.OK;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}