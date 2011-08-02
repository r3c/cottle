using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
            TextWriter  writer;

            parser = new Parser ();
            parser.Functions["count"] = this.ParseFunctionCount;
            parser.Functions["equal"] = this.ParseFunctionEqual;
            parser.Functions["greater"] = this.ParseFunctionGreater;
            parser.Functions["greaterequal"] = this.ParseFunctionGreaterEqual;
            parser.Functions["lower"] = this.ParseFunctionLower;
            parser.Functions["lowerequal"] = this.ParseFunctionLowerEqual;
            parser.Functions["match"] = this.ParseFunctionMatch;

            try
            {
                document = parser.Parse (new StringReader (this.textBoxInput.Text));
                
                this.FillValues (document.Values);

                writer = new StringWriter ();

                document.Debug (writer);

                this.textBoxDebug.Text = writer.ToString ();

                writer = new StringWriter ();

                document.Print (writer);

                this.textBoxPrint.Text = writer.ToString ();

                this.textBoxResult.BackColor = Color.LightGreen;
                this.textBoxResult.Text = "OK";
            }
            catch (Exception ex)
            {
                this.textBoxResult.BackColor = Color.LightSalmon;
                this.textBoxResult.Text = ex.Message;
            }
        }

        private void    FillValues (IDictionary<string, IValue> values)
        {
            Dictionary<string, IValue>  alertMessages = new Dictionary<string, IValue> ();
            Dictionary<string, IValue>  alertParams = new Dictionary<string, IValue> ();
            Dictionary<string, IValue>  alertTags = new Dictionary<string, IValue> ();
            string                      dateTime = DateTime.Now.ToString (CultureInfo.InvariantCulture);
            Random                      random = new Random ();

            for (int i = 0; i < 10; ++i)
            {
                alertMessages.Add (i.ToString (), new DictionaryValue (new Dictionary<string, IValue>
                {
                    {"contents",    new StringValue ("Contents for sample message #" + i)},
                    {"date_create", new StringValue (dateTime)},
                    {"date_gather", new StringValue (dateTime)},
                    {"origin",      new StringValue ("Sender")},
                    {"subject",     new StringValue ("Subject for sample message #" + i)}
                }));
            }

            for (int i = 0; i < 5; ++i)
            {
                alertParams.Add ("param" + i, new DictionaryValue (new Dictionary<string, IValue>
                {
                    {"value" + i + ".1",    new NumberValue (random.Next ())},
                    {"value" + i + ".2",    new NumberValue (random.Next ())},
                    {"value" + i + ".3",    new NumberValue (random.Next ())}
                }));
            }

            for (int i = 0; i < 5; ++i)
            {
                alertTags.Add (i.ToString (), new StringValue ("tag" + i));
            }

            values.Add ("messages", new DictionaryValue (alertMessages));
            values.Add ("params", new DictionaryValue (alertParams));
            values.Add ("tags", new DictionaryValue (alertTags));
        }

        private IValue  ParseFunctionCount (Scope scope, IList<IValue> arguments)
        {
            if (arguments.Count < 1)
                return new NumberValue (0);

            return new NumberValue (arguments[0].Children.Count);
        }

        private IValue  ParseFunctionEqual (Scope scope, IList<IValue> arguments)
        {
            string  compare;
            int     i;

            if (arguments.Count < 1)
                return new BooleanValue (false);

            compare = arguments[0].AsString;

            for (i = 1; i < arguments.Count; ++i)
                if (string.Compare (arguments[i].AsString, compare) != 0)
                    return new BooleanValue (false);

            return new BooleanValue (true);
        }

        private IValue  ParseFunctionGreater (Scope scope, IList<IValue> arguments)
        {
            if (arguments.Count < 2)
                return new BooleanValue (false);

            return new BooleanValue (arguments[0].AsNumber > arguments[1].AsNumber);
        }

        private IValue  ParseFunctionGreaterEqual (Scope scope, IList<IValue> arguments)
        {
            if (arguments.Count < 2)
                return new BooleanValue (false);

            return new BooleanValue (arguments[0].AsNumber >= arguments[1].AsNumber);
        }

        private IValue  ParseFunctionLower (Scope scope, IList<IValue> arguments)
        {
            if (arguments.Count < 2)
                return new BooleanValue (false);

            return new BooleanValue (arguments[0].AsNumber < arguments[1].AsNumber);
        }

        private IValue  ParseFunctionLowerEqual (Scope scope, IList<IValue> arguments)
        {
            if (arguments.Count < 2)
                return new BooleanValue (false);

            return new BooleanValue (arguments[0].AsNumber <= arguments[1].AsNumber);
        }

        private IValue  ParseFunctionMatch (Scope scope, IList<IValue> arguments)
        {
            if (arguments.Count < 2)
                return new BooleanValue (false);

            try
            {
                return new BooleanValue (Regex.IsMatch (arguments[0].AsString, arguments[1].AsString));
            }
            catch
            {
                return new BooleanValue (false);
            }
        }
    }
}
