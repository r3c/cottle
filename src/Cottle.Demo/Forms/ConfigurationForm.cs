using System;
using System.Windows.Forms;
using Cottle.Demo.Serialization;

namespace Cottle.Demo.Forms
{
    public partial class ConfigurationForm : Form
    {
        public DocumentConfiguration Configuration { get; private set; }

        public ConfigurationForm(DocumentConfiguration configuration)
        {
            InitializeComponent();

            foreach (var name in TrimmerSerializer.TrimmerNames)
                comboBoxTrimmer.Items.Add(name);

            comboBoxTrimmer.SelectedIndex = TrimmerSerializer.GetIndex(configuration.Trimmer);
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
                Trimmer = TrimmerSerializer.GetFunction(comboBoxTrimmer.SelectedIndex)
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