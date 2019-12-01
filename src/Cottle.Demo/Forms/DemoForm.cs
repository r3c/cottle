﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cottle.Values;

namespace Cottle.Demo.Forms
{
    public partial class DemoForm : Form
    {
        private const string Autoload = "autoload.ctv";

        private DocumentConfiguration _configuration;

        public DemoForm()
        {
            InitializeComponent();

            treeViewContext.Nodes.Add(new TreeNode("Cottle Context (right click here)", 6, 6));

            if (File.Exists(DemoForm.Autoload))
                StateLoad(DemoForm.Autoload, false);
        }

        private void buttonConfiguration_Click(object sender, EventArgs e)
        {
            using (var form = new ConfigurationForm(_configuration))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    _configuration = form.Configuration;
            }
        }

        private void buttonEvaluate_Click(object sender, EventArgs e)
        {
            var result = Document.CreateDefault(textBoxTemplate.Text, _configuration);

            if (!result.Success)
            {
                DisplayResult(result);

                return;
            }

            var symbols = new Dictionary<Value, Value>();

            foreach (TreeNode root in treeViewContext.Nodes)
            {
                foreach (var pair in ValuesBuild(root.Nodes))
                    symbols[pair.Key] = pair.Value;
            }

            DisplayOutput(result.Document.Render(Context.CreateBuiltin(symbols)));
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = @"Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                StateLoad(dialog.FileName, true);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = @"Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                StateSave(dialog.FileName);
        }

        private void toolStripMenuItemMoveDown_Click(object sender, EventArgs e)
        {
            if (!(contextMenuStripTree.Tag is TreeNode node1) || node1.Parent == null || node1.NextNode == null)
                return;

            var collection = node1.Parent.Nodes;
            var node2 = node1.NextNode;

            var index1 = node1.Index;
            var index2 = node2.Index;

            node2.Remove();
            collection.Insert(index1, node2);
            node1.Remove();
            collection.Insert(index2, node1);

            treeViewContext.SelectedNode = node1;
        }

        private void toolStripMenuItemMoveUp_Click(object sender, EventArgs e)
        {
            if (!(contextMenuStripTree.Tag is TreeNode node1) || node1.Parent == null || node1.PrevNode == null)
                return;

            var collection = node1.Parent.Nodes;
            var node2 = node1.PrevNode;

            var index1 = node1.Index;
            var index2 = node2.Index;

            node1.Remove();
            collection.Insert(index2, node1);
            node2.Remove();
            collection.Insert(index1, node2);

            treeViewContext.SelectedNode = node1;
        }

        private void toolStripMenuItemNodeClone_Click(object sender, EventArgs e)
        {
            if (contextMenuStripTree.Tag is TreeNode node)
                node.Parent?.Nodes.Insert(node.Index + 1, DemoForm.NodeClone(node));
        }

        private void toolStripMenuItemNodeCreate_Click(object sender, EventArgs e)
        {
            if (!(contextMenuStripTree.Tag is TreeNode node))
                return;

            new NodeForm(null, (key, value) =>
            {
                var child = DemoForm.NodeCreate(key, value);

                node.Nodes.Add(child);
                node.Expand();
            }).ShowDialog(this);
        }

        private void toolStripMenuItemNodeDelete_Click(object sender, EventArgs e)
        {
            if (contextMenuStripTree.Tag is TreeNode node && node.Parent != null)
                node.Remove();
        }

        private void toolStripMenuItemNodeUpdate_Click(object sender, EventArgs e)
        {
            if (!(contextMenuStripTree.Tag is TreeNode node))
                return;

            new NodeForm(node.Tag as NodeData, (key, value) => DemoForm.NodeAssign(node, key, value)).ShowDialog(this);
        }

        private void toolStripMenuItemTreeCollapse_Click(object sender, EventArgs e)
        {
            treeViewContext.CollapseAll();
        }

        private void toolStripMenuItemTreeExpand_Click(object sender, EventArgs e)
        {
            treeViewContext.ExpandAll();
        }

        private void contextMenuStripTree_Opening(object sender, CancelEventArgs e)
        {
            var node = treeViewContext.SelectedNode;
            var isMapValue = node?.Tag is NodeData data && data.Value.Type == ValueContent.Map;
            var isRoot = node?.Parent == null;

            contextMenuStripTree.Tag = node;
            toolStripMenuItemNodeClone.Enabled = !isRoot;
            toolStripMenuItemNodeCreate.Enabled = isRoot || isMapValue;
            toolStripMenuItemNodeDelete.Enabled = !isRoot;
            toolStripMenuItemNodeUpdate.Enabled = !isRoot;
            toolStripMenuItemMoveDown.Enabled = !isRoot && node.NextNode != null;
            toolStripMenuItemMoveUp.Enabled = !isRoot && node.PrevNode != null;
        }

        private void DisplayOutput(string text)
        {
            textBoxOutput.BackColor = Color.DarkSeaGreen;
            textBoxOutput.Text = text;
        }

        private void DisplayResult(DocumentResult result)
        {
            var messages = result.Reports.Select(r => $"{r.Message} ({r.Severity})");

            textBoxOutput.BackColor = Color.DarkSalmon;
            textBoxOutput.Text = string.Join(Environment.NewLine, messages);

            var firstReport = result.Reports.FirstOrDefault();

            textBoxTemplate.SelectionStart = Math.Max(Math.Min(firstReport.Offset, textBoxTemplate.Text.Length - 1), 0);
            textBoxTemplate.SelectionLength = firstReport.Length;
            textBoxTemplate.Focus();
        }

        private static void NodeAssign(TreeNode node, string key, Value value)
        {
            var data = new NodeData(key, value);

            node.ImageIndex = data.ImageIndex;
            node.SelectedImageIndex = data.ImageIndex;
            node.Tag = data;

            switch (value.Type)
            {
                case ValueContent.Map:
                    node.Text = key;

                    break;

                default:
                    node.Text = $@"{key} = {value}";

                    break;
            }
        }

        private static TreeNode NodeClone(TreeNode node)
        {
            TreeNode copy;

            if (node.Tag is NodeData data)
            {
                copy = DemoForm.NodeCreate(data.Key, data.Value);

                if (data.Value.Type == ValueContent.Map)
                {
                    foreach (TreeNode child in node.Nodes)
                        copy.Nodes.Add(DemoForm.NodeClone(child));
                }

                copy.Expand();
            }
            else
                copy = new TreeNode();

            return copy;
        }

        private static TreeNode NodeCreate(string key, Value value)
        {
            var node = new TreeNode();

            switch (value.Type)
            {
                case ValueContent.Boolean:
                    DemoForm.NodeAssign(node, key, new BooleanValue(value.AsBoolean));

                    return node;

                case ValueContent.Map:
                    var range = new TreeNode[value.Fields.Count];
                    var i = 0;

                    foreach (var pair in value.Fields)
                        range[i++] = DemoForm.NodeCreate(pair.Key.AsString, pair.Value);

                    node.Nodes.AddRange(range);

                    DemoForm.NodeAssign(node, key, MapValue.Empty);

                    return node;

                case ValueContent.Number:
                    DemoForm.NodeAssign(node, key, new NumberValue(value.AsNumber));

                    return node;

                case ValueContent.String:
                    DemoForm.NodeAssign(node, key, new StringValue(value.AsString));

                    return node;

                default:
                    DemoForm.NodeAssign(node, key, VoidValue.Instance);

                    return node;
            }
        }

        private void StateLoad(string path, bool dialog)
        {
            if (treeViewContext.Nodes.Count < 1)
                return;

            var root = treeViewContext.Nodes[0];

            root.Nodes.Clear();

            try
            {
                using (var reader = new BinaryReader(new FileStream(path, FileMode.Open), Encoding.UTF8))
                {
                    var values = new Dictionary<string, Value>();
                    var version = reader.ReadInt32();

                    if (version < 1 || version > 2)
                    {
                        MessageBox.Show(this, @"Incompatible file format");

                        return;
                    }

                    if (ValueAccessor.Load(reader, values))
                    {
                        foreach (var pair in values)
                            root.Nodes.Add(DemoForm.NodeCreate(pair.Key, pair.Value));
                    }

                    var blockBegin = reader.ReadString();
                    var blockContinue = reader.ReadString();
                    var blockEnd = reader.ReadString();
                    var trimmer = TrimmerCollection.GetTrimmerFunction(version > 1
                        ? reader.ReadInt32()
                        : TrimmerCollection.DefaultIndex);

                    _configuration = new DocumentConfiguration
                    {
                        BlockBegin = blockBegin,
                        BlockContinue = blockContinue,
                        BlockEnd = blockEnd,
                        Trimmer = trimmer
                    };

                    textBoxTemplate.Text = reader.ReadString();
                }

                root.ExpandAll();

                if (dialog)
                {
                    MessageBox.Show(this, $@"State successfully loaded from ""{path}"".", @"File save successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show(this, $@"Cannot open input file ""{path}""", @"File load error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void StateSave(string path)
        {
            var values = new Dictionary<string, Value>();

            foreach (TreeNode root in treeViewContext.Nodes)
            {
                foreach (var pair in ValuesBuild(root.Nodes))
                    values[pair.Key.AsString] = pair.Value;
            }

            try
            {
                using (var writer = new BinaryWriter(new FileStream(path, FileMode.Create), Encoding.UTF8))
                {
                    writer.Write(2);

                    ValueAccessor.Save(writer, values);

                    writer.Write(_configuration.BlockBegin);
                    writer.Write(_configuration.BlockContinue);
                    writer.Write(_configuration.BlockEnd);
                    writer.Write(TrimmerCollection.GetTrimmerIndex(_configuration.Trimmer));
                    writer.Write(textBoxTemplate.Text);
                }

                MessageBox.Show(this, $@"State successfully saved to ""{path}"".", @"File save successful",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(this, $@"Cannot open output file ""{path}""", @"File save error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private List<KeyValuePair<Value, Value>> ValuesBuild(TreeNodeCollection nodes)
        {
            var collection = new List<KeyValuePair<Value, Value>>(nodes.Count);

            foreach (TreeNode node in nodes)
            {
                if (!(node.Tag is NodeData data))
                    continue;

                switch (data.Value.Type)
                {
                    case ValueContent.Map:
                        collection.Add(new KeyValuePair<Value, Value>(data.Key, ValuesBuild(node.Nodes)));

                        break;

                    default:
                        collection.Add(new KeyValuePair<Value, Value>(data.Key, data.Value));

                        break;
                }
            }

            return collection;
        }
    }
}