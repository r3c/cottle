using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Cottle;
using Cottle.Commons;
using Cottle.Exceptions;
using Cottle.Values;

namespace   Demo
{
    public partial class    DemoForm : Form
    {
        #region Constants

        private const string    AUTOLOAD = "autoload.ctv";

        #endregion

        #region Constructors

        public  DemoForm ()
        {
            InitializeComponent ();

            this.treeViewValue.Nodes.Add (new TreeNode ("Document", (int)ValueContent.Array, (int)ValueContent.Array));

            if (File.Exists (DemoForm.AUTOLOAD))
                this.ValuesLoad (DemoForm.AUTOLOAD, false);
        }

        #endregion

        #region Methods / Listeners

        private void    buttonDemo_Click (object sender, EventArgs e)
        {
            Document    document;
            Scope       scope;

            try
            {
                try
                {
                    document = new Document (new StringReader (this.textBoxInput.Text));
                    scope = new Scope ();

                    CommonFunctions.Assign (scope);

                    foreach (TreeNode root in this.treeViewValue.Nodes)
                    {
                        foreach (KeyValuePair<Value, Value> pair in this.ValuesBuild (root.Nodes))
                            scope.Set (pair.Key, pair.Value, ScopeMode.Closest);
                    }

                    this.textBoxPrint.Text = document.Render (scope);

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

        private void    toolStripMenuItemFileLoad_Click (object sender, EventArgs e)
        {
            OpenFileDialog  dialog = new OpenFileDialog ();

            dialog.Filter = "Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*";

            if (dialog.ShowDialog (this) == DialogResult.OK)
                this.ValuesLoad (dialog.FileName, true);
        }

        private void    toolStripMenuItemFileSave_Click (object sender, EventArgs e)
        {
            SaveFileDialog  dialog = new SaveFileDialog ();

            dialog.Filter = "Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*";

            if (dialog.ShowDialog (this) == DialogResult.OK)
                this.ValuesSave (dialog.FileName);
        }

        private void    toolStripMenuItemMoveDown_Click (object sender, EventArgs e)
        {
            TreeNodeCollection  collection;
            int                 index1;
            int                 index2;
            TreeNode            node1 = this.contextMenuStripTree.Tag as TreeNode;
            TreeNode            node2;

            if (node1 != null && node1.Parent != null && node1.NextNode != null)
            {
                collection = node1.Parent.Nodes;
                node2 = node1.NextNode;

                index1 = node1.Index;
                index2 = node2.Index;

                node2.Remove ();
                collection.Insert (index1, node2);
                node1.Remove ();
                collection.Insert (index2, node1);

                this.treeViewValue.SelectedNode = node1;
            }
        }

        private void    toolStripMenuItemMoveUp_Click (object sender, EventArgs e)
        {
            TreeNodeCollection  collection;
            int                 index1;
            int                 index2;
            TreeNode            node1 = this.contextMenuStripTree.Tag as TreeNode;
            TreeNode            node2;

            if (node1 != null && node1.Parent != null && node1.PrevNode != null)
            {
                collection = node1.Parent.Nodes;
                node2 = node1.PrevNode;

                index1 = node1.Index;
                index2 = node2.Index;

                node1.Remove ();
                collection.Insert (index2, node1);
                node2.Remove ();
                collection.Insert (index1, node2);

                this.treeViewValue.SelectedNode = node1;
            }
        }

        private void    toolStripMenuItemNodeClone_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;

            if (node != null && node.Parent != null)
                node.Parent.Nodes.Insert (node.Index + 1, this.NodeClone (node));
        }

        private void    toolStripMenuItemNodeCreate_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;

            if (node != null)
            {
                new NodeForm (null, delegate (string key, Value value)
                {
                    TreeNode    child = this.NodeCreate (key, value);

                    node.Nodes.Add (child);
                    node.Expand ();
                }).Show (this);
            }
        }

        private void    toolStripMenuItemNodeDelete_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;

            if (node != null && node.Parent != null)
                node.Remove ();
        }

        private void    toolStripMenuItemNodeUpdate_Click (object sender, EventArgs e)
        {
            TreeNode    node = this.contextMenuStripTree.Tag as TreeNode;

            if (node != null)
                new NodeForm (node.Tag as NodeData, (key, value) => this.NodeAssign (node, key, value)).Show (this);
        }

        private void    toolStripMenuItemTreeCollapse_Click (object sender, EventArgs e)
        {
            this.treeViewValue.CollapseAll ();
        }

        private void    toolStripMenuItemTreeExpand_Click (object sender, EventArgs e)
        {
            this.treeViewValue.ExpandAll ();
        }

        private void    contextMenuStripTree_Opening (object sender, CancelEventArgs e)
        {
            TreeNode    node = this.treeViewValue.SelectedNode;
            NodeData    data = node != null ? node.Tag as NodeData : null;

            this.toolStripMenuItemNodeClone.Enabled = node != null && node.Parent != null;
            this.toolStripMenuItemNodeCreate.Enabled = node != null && node.Parent == null || (data != null && data.Value.Type == ValueContent.Array);
            this.toolStripMenuItemNodeDelete.Enabled = node != null && node.Parent != null;
            this.toolStripMenuItemMoveDown.Enabled = node != null && node.Parent != null && node != null && node.NextNode != null;
            this.toolStripMenuItemMoveUp.Enabled = node != null && node.Parent != null && node != null && node.PrevNode != null;
            this.toolStripMenuItemNodeUpdate.Enabled = node != null && node.Parent != null;

            this.contextMenuStripTree.Tag = node;
        }

        #endregion

        #region Methods / Private

        private void    NodeAssign (TreeNode node, string key, Value value)
        {
            NodeData    data = new NodeData (key, value);

            node.ImageIndex = data.ImageIndex;
            node.SelectedImageIndex = data.ImageIndex;
            node.Tag = data;

            switch (value.Type)
            {
                case ValueContent.Array:
                    node.Text = string.Format ("{0}", key);

                    break;

                default:
                    node.Text = string.Format ("{0} = {1}", key, value);

                    break;
            }
        }

        private TreeNode    NodeClone (TreeNode node)
        {
            NodeData    data = node.Tag as NodeData;
            TreeNode    copy;

            if (data != null)
            {
                copy = this.NodeCreate (data.Key, data.Value);

                if (data.Value.Type == ValueContent.Array)
                {
                    foreach (TreeNode child in node.Nodes)
                        copy.Nodes.Add (this.NodeClone (child));
                }

                copy.Expand ();
            }
            else
                copy = new TreeNode ();

            return copy;
        }

        private TreeNode    NodeCreate (string key, Value value)
        {
            TreeNode    node = new TreeNode ();

            switch (value.Type)
            {
                case ValueContent.Array:
                    node.Nodes.AddRange (Array.ConvertAll (value.Fields, (p) => this.NodeCreate (p.Key.AsString, p.Value)));

                    this.NodeAssign (node, key, new ArrayValue ());

                    return node;

                case ValueContent.Boolean:
                    this.NodeAssign (node, key, new BooleanValue (value.AsBoolean));

                    return node;

                case ValueContent.Number:
                    this.NodeAssign (node, key, new NumberValue (value.AsNumber));

                    return node;

                case ValueContent.String:
                    this.NodeAssign (node, key, new StringValue (value.AsString));

                    return node;

                default:
                    this.NodeAssign (node, key, UndefinedValue.Instance);

                    return node;
            }
        }

        private List<KeyValuePair<Value, Value>>    ValuesBuild (TreeNodeCollection nodes)
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
                        case ValueContent.Array:
                            collection.Add (new KeyValuePair<Value, Value> (data.Key, this.ValuesBuild (node.Nodes)));

                            break;

                        default:
                            collection.Add (new KeyValuePair<Value, Value> (data.Key, data.Value));

                            break;
                    }
                }
            }

            return collection;
        }

        private void    ValuesLoad (string path, bool dialog)
        {
            TreeNode                    root;
            Dictionary<string, Value>   values;

            if (this.treeViewValue.Nodes.Count < 1)
                return;

            root = this.treeViewValue.Nodes[0];
            root.Nodes.Clear ();

            try
            {
                using (Stream stream = new FileStream (path, FileMode.Open))
                {
                    values = new Dictionary<string, Value> ();

                    if (CommonTools.ValuesLoad (new BinaryReader (stream, Encoding.UTF8), values))
                    {
                        foreach (KeyValuePair<string, Value> pair in values)
                            root.Nodes.Add (this.NodeCreate (pair.Key, pair.Value));
                    }
                }

                root.ExpandAll ();

                if (dialog)
                    MessageBox.Show (this, string.Format ("Values successfully loaded from \"{0}\".", path), "File save successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show (this, string.Format ("Cannot open input file \"{0}\"", path), "File load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void    ValuesSave (string path)
        {
            Dictionary<string, Value>   values = new Dictionary<string, Value> ();

            foreach (TreeNode root in this.treeViewValue.Nodes)
            {
                foreach (KeyValuePair<Value, Value> pair in this.ValuesBuild (root.Nodes))
                    values[pair.Key.AsString] = pair.Value;
            }

            try
            {
                using (Stream stream = new FileStream (path, FileMode.Create))
                {
                    CommonTools.ValuesSave (new BinaryWriter (stream, Encoding.UTF8), values);
                }

                MessageBox.Show (this, string.Format ("Values successfully saved to \"{0}\".", path), "File save successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show (this, string.Format ("Cannot open output file \"{0}\"", path), "File save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
