using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Cottle;
using Cottle.Exceptions;
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

        private void    FillFunctions (IDictionary<string, IValue> values)
        {
            values.Add ("add", new FunctionValue (new Function (Callbacks.Add, 2)));
            values.Add ("contains", new FunctionValue (new Function (Callbacks.Contains, 1, -1)));
            values.Add ("count", new FunctionValue (new Function (Callbacks.Count, 1)));
            values.Add ("div", new FunctionValue (new Function (Callbacks.Div, 2)));
            values.Add ("equal", new FunctionValue (new Function (Callbacks.Equal, 1, -1)));
            values.Add ("gequal", new FunctionValue (new Function (Callbacks.GreaterEqual, 2)));
            values.Add ("greater", new FunctionValue (new Function (Callbacks.Greater, 2)));
            values.Add ("lequal", new FunctionValue (new Function (Callbacks.LowerEqual, 2)));
            values.Add ("lower", new FunctionValue (new Function (Callbacks.Lower, 2)));
            values.Add ("match", new FunctionValue (new Function (Callbacks.Match, 2)));
            values.Add ("mod", new FunctionValue (new Function (Callbacks.Mod, 2)));
            values.Add ("mul", new FunctionValue (new Function (Callbacks.Mul, 2)));
            values.Add ("slice", new FunctionValue (new Function (Callbacks.Slice, 2, 3)));
            values.Add ("sub", new FunctionValue (new Function (Callbacks.Sub, 2)));
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
