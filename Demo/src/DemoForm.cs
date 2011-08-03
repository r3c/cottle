using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Cottle;
using Cottle.Values;

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
                document = parser.Parse (new StringReader (this.textBoxInput.Text));

                this.FillFunctions (document.Values);
                this.FillValues (document.Values);

                this.textBoxDebug.Text = document.Debug ();
                this.textBoxPrint.Text = document.Print ();

                this.textBoxResult.BackColor = Color.LightGreen;
                this.textBoxResult.Text = "OK";
            }
            catch (Exception ex)
            {
                this.textBoxResult.BackColor = Color.LightSalmon;
                this.textBoxResult.Text = ex.Message;
            }
        }

        private void    FillFunctions (IDictionary<string, IValue> values)
        {
            values.Add ("add", new FunctionValue (Callbacks.Add));
            values.Add ("contains", new FunctionValue (Callbacks.Contains));
            values.Add ("count", new FunctionValue (Callbacks.Count));
            values.Add ("div", new FunctionValue (Callbacks.Div));
            values.Add ("equal", new FunctionValue (Callbacks.Equal));
            values.Add ("gequal", new FunctionValue (Callbacks.GreaterEqual));
            values.Add ("greater", new FunctionValue (Callbacks.Greater));
            values.Add ("lequal", new FunctionValue (Callbacks.LowerEqual));
            values.Add ("lower", new FunctionValue (Callbacks.Lower));
            values.Add ("match", new FunctionValue (Callbacks.Match));
            values.Add ("mod", new FunctionValue (Callbacks.Mod));
            values.Add ("mul", new FunctionValue (Callbacks.Mul));
            values.Add ("slice", new FunctionValue (Callbacks.Slice));
            values.Add ("sub", new FunctionValue (Callbacks.Sub));
        }

        private void    FillValues (IDictionary<string, IValue> values)
        {
            Dictionary<IValue, IValue>  alertMessages = new Dictionary<IValue, IValue> ();
            Dictionary<IValue, IValue>  alertParams = new Dictionary<IValue, IValue> ();
            Dictionary<IValue, IValue>  alertTags = new Dictionary<IValue, IValue> ();
            string                      dateTime = DateTime.Now.ToString (CultureInfo.InvariantCulture);
            Random                      random = new Random ();

            for (int i = 0; i < 10; ++i)
            {
                alertMessages.Add (new NumberValue (i), new ArrayValue (new Dictionary<IValue, IValue>
                {
                    {new StringValue ("contents"),     new StringValue ("Contents for sample message #" + i)},
                    {new StringValue ("date_create"),  new StringValue (dateTime)},
                    {new StringValue ("date_gather"),  new StringValue (dateTime)},
                    {new StringValue ("origin"),       new StringValue ("Sender")},
                    {new StringValue ("subject"),      new StringValue ("Subject for sample message #" + i)}
                }));
            }

            for (int i = 0; i < 5; ++i)
            {
                alertParams.Add (new StringValue ("param #" + i), new ArrayValue (new Dictionary<IValue, IValue>
                {
                    {new StringValue ("value" + i + ".1"), new NumberValue (random.Next ())},
                    {new StringValue ("value" + i + ".2"), new NumberValue (random.Next ())},
                    {new StringValue ("value" + i + ".3"), new NumberValue (random.Next ())}
                }));
            }

            for (int i = 0; i < 5; ++i)
            {
                alertTags.Add (new StringValue ("tag #" + i), new NumberValue (i));
            }

            values.Add ("messages", new ArrayValue (alertMessages));
            values.Add ("params", new ArrayValue (alertParams));
            values.Add ("tags", new ArrayValue (alertTags));
        }
    }
}
