using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Cottle;
using Cottle.Exceptions;
using Cottle.Commons;

namespace   Demo
{
    public partial class    DemoForm : Form
    {
        public  DemoForm ()
        {
            InitializeComponent ();
        }

        private void    buttonDemo_Click (object sender, EventArgs e)
        {
            Document    document;
            Parser      parser;

            parser = new Parser ();

            try
            {
                try
                {
                    document = parser.Parse (new StringReader (this.textBoxInput.Text));

                    CommonFunctions.Assign (document);

                    this.FillDocument (document.Values);

                    this.textBoxPrint.Text = document.Print ();

                    this.textBoxResult.BackColor = Color.LightGreen;
                    this.textBoxResult.Text = "OK";
                }
                catch (UnexpectedException ex)
                {
                    this.textBoxInput.SelectionStart = Math.Max (ex.Index - ex.Value.Length - 1, 0);
                    this.textBoxInput.SelectionLength = ex.Value.Length;
                    this.textBoxInput.Focus ();

                    throw;
                }
                catch (UnknownException ex)
                {
                    this.textBoxInput.SelectionStart = Math.Max (ex.Index - 1, 0);
                    this.textBoxInput.SelectionLength = 1;
                    this.textBoxInput.Focus ();

                    throw;
                }
            }
            catch (Exception ex)
            {
                this.textBoxResult.BackColor = Color.LightSalmon;
                this.textBoxResult.Text = ex.Message;
            }
        }
#if false
        private void    FillTree ()
        {
            this.treeViewData.Nodes.Clear ();
        }
#endif
        private void    FillDocument (IDictionary<string, Value> values)
        {
            Dictionary<Value, Value>    alertMessages = new Dictionary<Value, Value> ();
            Dictionary<Value, Value>    alertParams = new Dictionary<Value, Value> ();
            Dictionary<Value, Value>    alertTags = new Dictionary<Value, Value> ();
            string                      dateTime = DateTime.Now.ToString (CultureInfo.InvariantCulture);
            Random                      random = new Random ();

            for (int i = 0; i < 10; ++i)
            {
                alertMessages.Add (i, new Dictionary<Value, Value>
                {
                    {"contents",    "Contents for sample message #" + i},
                    {"date_create", dateTime},
                    {"date_gather", dateTime},
                    {"origin",      "Sender"},
                    {"subject",     "Subject for sample message #" + i}
                });
            }

            for (int i = 0; i < 5; ++i)
            {
                alertParams.Add ("param #" + i, new Dictionary<Value, Value>
                {
                    {"value" + i + ".1",    random.Next ()},
                    {"value" + i + ".2",    random.Next ()},
                    {"value" + i + ".3",    random.Next ()}
                });
            }

            for (int i = 0; i < 5; ++i)
            {
                alertTags.Add ("tag #" + i, i);
            }

            values.Add ("messages", alertMessages);
            values.Add ("params", alertParams);
            values.Add ("tags", alertTags);
        }
    }
}
