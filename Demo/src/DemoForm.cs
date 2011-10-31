using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Cottle;
using Cottle.Exceptions;
using Cottle.Commons;
using Cottle.Values;

namespace   Demo
{
    public partial class    DemoForm : Form
    {
        #region Attributes

        private TreeNode    root = new TreeNode ("Document", (int)Value.DataType.ARRAY, (int)Value.DataType.ARRAY);

        #endregion

        #region Constructors

        public  DemoForm ()
        {
            InitializeComponent ();

            this.treeViewValue.Nodes.Add (this.root);
            this.treeViewValue.ExpandAll ();
        }

        #endregion

        #region Methods / Listeners

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

                    foreach (KeyValuePair<Value, Value> pair in this.ReadValues (this.root.Nodes))
                        document.Values[pair.Key.AsString] = pair.Value;

                    this.textBoxPrint.Text = document.Print ();

                    this.textBoxResult.BackColor = Color.LightGreen;
                    this.textBoxResult.Text = "OK";
                }
                catch (UnexpectedException ex)
                {
                    this.textBoxInput.SelectionStart = Math.Max (ex.Index - ex.Current.Length - 1, 0);
                    this.textBoxInput.SelectionLength = ex.Current.Length;
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

        private void    toolStripMenuItemCreate_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;

            if (node == null)
                return;

            new NodeForm (delegate (TreeNode child)
            {
                node.Nodes.Add (child);
                node.Expand ();
            }).Show (this);
        }

        private void    toolStripMenuItemDelete_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;

            if (node != null && node != this.root)
                node.Remove ();
        }

        private void    toolStripMenuItemMoveDown_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;
            int         swapIndex;
            TreeNode    swapNode;

            if (node == null || node == this.root || node.NextNode == null)
                return;
throw new NotImplementedException ();
            swapIndex = node.NextNode.Index;
            swapNode = node.Parent.Nodes[node.Index];

            node.Parent.Nodes[node.Index] = node.NextNode;
            node.Parent.Nodes[swapIndex] = swapNode;

        }

        private void    toolStripMenuItemMoveUp_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;
            int         swapIndex;
            TreeNode    swapNode;

            if (node == null || node == this.root || node.PrevNode == null)
                return;
throw new NotImplementedException ();
            swapIndex = node.PrevNode.Index;
            swapNode = node.Parent.Nodes[node.Index];

            node.Parent.Nodes[node.Index] = node.PrevNode;
            node.Parent.Nodes[swapIndex] = swapNode;
        }

        private void    treeViewValue_NodeMouseClick (object sender, TreeNodeMouseClickEventArgs e)
        {
            NodeData    data = e.Node.Tag as NodeData;

            if (e.Button != MouseButtons.Right)
                return;

            this.toolStripMenuItemCreate.Enabled = e.Node == this.root || (data != null && data.Value.Type == Value.DataType.ARRAY);
            this.toolStripMenuItemDelete.Enabled = e.Node != this.root;
            this.toolStripMenuItemMoveDown.Enabled = e.Node != this.root && e.Node.NextNode != null;
            this.toolStripMenuItemMoveUp.Enabled = e.Node != this.root && e.Node.PrevNode != null;

            this.contextMenuStripTree.Tag = e.Node;
            this.contextMenuStripTree.Show (this.treeViewValue, e.X, e.Y);
        }

        #endregion

        #region Methods / Private

        private KeyValuePair<Value, Value>[]    ReadValues (TreeNodeCollection nodes)
        {
            List<KeyValuePair<Value, Value>>    collection = new List<KeyValuePair<Value,Value>> (nodes.Count);
            NodeData                            data;

            foreach (TreeNode node in nodes)
            {
                data = node.Tag as NodeData;

                if (data != null)
                {
                    switch (data.Value.Type)
                    {
                        case Value.DataType.ARRAY:
                            collection.Add (new KeyValuePair<Value, Value> (data.Key, this.ReadValues (node.Nodes)));

                            break;

                        default:
                            collection.Add (new KeyValuePair<Value, Value> (data.Key, data.Value));

                            break;
                    }
                }
            }

            return collection.ToArray ();
        }

        /*private void    SetValues (Document document)
        {
            Dictionary<Value, Value>    alertMessages = new Dictionary<Value, Value> ();
            Dictionary<Value, Value>    alertProps = new Dictionary<Value, Value> ();
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
                alertProps.Add ("prop #" + i, new Dictionary<Value, Value>
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

            document.Values.Add ("messages", alertMessages);
            document.Values.Add ("props", alertProps);
            document.Values.Add ("tags", alertTags);
        }*/

        #endregion
    }
}
