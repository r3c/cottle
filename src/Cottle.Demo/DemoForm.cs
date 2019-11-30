using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Cottle.Documents;
using Cottle.Exceptions;
using Cottle.Settings;
using Cottle.Values;

namespace Cottle.Demo
{
	public partial class DemoForm : Form
	{
#region Constants

		private const string AUTOLOAD = "autoload.ctv";

#endregion

#region Attributes / Instance

		private SettingForm.Parameters parameters = new SettingForm.Parameters
		{
			BlockBegin = DefaultSetting.Instance.BlockBegin,
			BlockContinue = DefaultSetting.Instance.BlockContinue,
			BlockEnd = DefaultSetting.Instance.BlockEnd,
			TrimmerIndex = TrimmerCollection.DefaultIndex
		};

#endregion

#region Constructors

		public DemoForm ()
		{
			InitializeComponent ();

			this.treeViewValue.Nodes.Add (new TreeNode ("Cottle Store", 6, 6));

			if (File.Exists (DemoForm.AUTOLOAD))
				this.StateLoad (DemoForm.AUTOLOAD, false);
		}

#endregion

#region Methods / Listeners

		private void buttonClean_Click (object sender, EventArgs e)
		{
			SimpleDocument document;
			ISetting setting;

			setting = this.SettingCreate ();

			try
			{
				document = new SimpleDocument (this.textBoxInput.Text, setting);

				this.DisplayText (document.Source ());
			}
			catch (ParseException exception)
			{
				this.DisplayError (exception);
			}
		}

		private void buttonEvaluate_Click (object sender, EventArgs e)
		{
			try
			{
				var document = new SimpleDocument (this.textBoxInput.Text, this.SettingCreate ());
				var symbols = new Dictionary<Value, Value>();

				foreach (TreeNode root in this.treeViewValue.Nodes)
				{
					foreach (var pair in this.ValuesBuild (root.Nodes))
						symbols[pair.Key] = pair.Value;
				}

				this.DisplayText(document.Render(Context.CreateBuiltin(symbols)));
			}
			catch (ParseException exception)
			{
				this.DisplayError (exception);
			}
		}

		private void buttonSave_Click (object sender, EventArgs e)
		{
			ISetting setting = this.SettingCreate ();

			using (var dialog = new SaveFileDialog ())
			{
				dialog.AddExtension = true;
				dialog.DefaultExt = "dll";
				dialog.Filter = "DLL files (*.dll)|*.dll";
				dialog.OverwritePrompt = true;

				if (dialog.ShowDialog (this) == DialogResult.OK)
				{
					try
					{
						DynamicDocument.Save (new StringReader (this.textBoxInput.Text), setting, Path.GetFileNameWithoutExtension (dialog.FileName), Path.GetFileName (dialog.FileName));

						this.DisplayText ("File saved.");
					}
					catch (ParseException exception)
					{
						this.DisplayError (exception);
					}
				}
			}
		}

		private void toolStripMenuItemSetting_Click (object sender, EventArgs e)
		{
			Form form;

			form = new SettingForm ((p) => this.parameters = p, this.parameters);
			form.ShowDialog (this);
		}

		private void toolStripMenuItemFileLoad_Click (object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog ();

			dialog.Filter = "Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*";

			if (dialog.ShowDialog (this) == DialogResult.OK)
				this.StateLoad (dialog.FileName, true);
		}

		private void toolStripMenuItemFileSave_Click (object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog ();

			dialog.Filter = "Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*";

			if (dialog.ShowDialog (this) == DialogResult.OK)
				this.StateSave (dialog.FileName);
		}

		private void toolStripMenuItemMoveDown_Click (object sender, EventArgs e)
		{
			TreeNodeCollection collection;
			int index1;
			int index2;
			TreeNode node1 = this.contextMenuStripTree.Tag as TreeNode;
			TreeNode node2;

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

		private void toolStripMenuItemMoveUp_Click (object sender, EventArgs e)
		{
			TreeNodeCollection collection;
			int index1;
			int index2;
			TreeNode node1 = this.contextMenuStripTree.Tag as TreeNode;
			TreeNode node2;

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

		private void toolStripMenuItemNodeClone_Click (object sender, EventArgs e)
		{
			TreeNode node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null && node.Parent != null)
				node.Parent.Nodes.Insert (node.Index + 1, this.NodeClone (node));
		}

		private void toolStripMenuItemNodeCreate_Click (object sender, EventArgs e)
		{
			Form form;
			TreeNode node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null)
			{
				form = new NodeForm (null, delegate (string key, Value value)
				{
					TreeNode child = this.NodeCreate (key, value);

					node.Nodes.Add (child);
					node.Expand ();
				});

				form.ShowDialog (this);
			}
		}

		private void toolStripMenuItemNodeDelete_Click (object sender, EventArgs e)
		{
			TreeNode node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null && node.Parent != null)
				node.Remove ();
		}

		private void toolStripMenuItemNodeUpdate_Click (object sender, EventArgs e)
		{
			Form form;
			TreeNode node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null)
			{
				form = new NodeForm (node.Tag as NodeData, (key, value) => this.NodeAssign (node, key, value));
				form.ShowDialog (this);
			}
		}

		private void toolStripMenuItemTreeCollapse_Click (object sender, EventArgs e)
		{
			this.treeViewValue.CollapseAll ();
		}

		private void toolStripMenuItemTreeExpand_Click (object sender, EventArgs e)
		{
			this.treeViewValue.ExpandAll ();
		}

		private void contextMenuStripTree_Opening (object sender, CancelEventArgs e)
		{
			TreeNode node = this.treeViewValue.SelectedNode;
			NodeData data = node != null ? node.Tag as NodeData : null;

			this.toolStripMenuItemNodeClone.Enabled = node != null && node.Parent != null;
			this.toolStripMenuItemNodeCreate.Enabled =
 node != null && node.Parent == null || (data != null && data.Value.Type == ValueContent.Map);
			this.toolStripMenuItemNodeDelete.Enabled = node != null && node.Parent != null;
			this.toolStripMenuItemMoveDown.Enabled =
 node != null && node.Parent != null && node != null && node.NextNode != null;
			this.toolStripMenuItemMoveUp.Enabled =
 node != null && node.Parent != null && node != null && node.PrevNode != null;
			this.toolStripMenuItemNodeUpdate.Enabled = node != null && node.Parent != null;

			this.contextMenuStripTree.Tag = node;
		}

#endregion

#region Methods / Private

		private void DisplayError (ParseException exception)
		{
			int index;

			index = this.textBoxInput.GetFirstCharIndexFromLine (exception.Line - 1) + exception.Column;

			this.textBoxInput.SelectionStart =
 Math.Max (Math.Min (index - exception.Lexem.Length - 1, this.textBoxInput.Text.Length - 1), 0);
			this.textBoxInput.SelectionLength = exception.Lexem.Length;
			this.textBoxInput.Focus ();

			this.textBoxPrint.BackColor = Color.LightPink;
			this.textBoxPrint.Text = "Document error: " + exception.Message;
		}

		private void DisplayText (string text)
		{
			this.textBoxPrint.BackColor = Color.LightGreen;
			this.textBoxPrint.Text = text;
		}

		private void NodeAssign (TreeNode node, string key, Value value)
		{
			NodeData data = new NodeData (key, value);

			node.ImageIndex = data.ImageIndex;
			node.SelectedImageIndex = data.ImageIndex;
			node.Tag = data;

			switch (value.Type)
			{
				case ValueContent.Map:
					node.Text = string.Format (CultureInfo.InvariantCulture, "{0}", key);

					break;

				default:
					node.Text = string.Format (CultureInfo.InvariantCulture, "{0} = {1}", key, value);

					break;
			}
		}

		private TreeNode NodeClone (TreeNode node)
		{
			NodeData data = node.Tag as NodeData;
			TreeNode copy;

			if (data != null)
			{
				copy = this.NodeCreate (data.Key, data.Value);

				if (data.Value.Type == ValueContent.Map)
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

		private TreeNode NodeCreate (string key, Value value)
		{
			TreeNode node;
			TreeNode[] range;
			int i;

			node = new TreeNode ();

			switch (value.Type)
			{
				case ValueContent.Boolean:
					this.NodeAssign (node, key, new BooleanValue (value.AsBoolean));

					return node;

				case ValueContent.Map:
					range = new TreeNode[value.Fields.Count];
					i = 0;

					foreach (KeyValuePair<Value, Value> pair in value.Fields)
						range[i++] = this.NodeCreate (pair.Key.AsString, pair.Value);

					node.Nodes.AddRange (range);

					this.NodeAssign (node, key, MapValue.Empty);

					return node;

				case ValueContent.Number:
					this.NodeAssign (node, key, new NumberValue (value.AsNumber));

					return node;

				case ValueContent.String:
					this.NodeAssign (node, key, new StringValue (value.AsString));

					return node;

				default:
					this.NodeAssign (node, key, VoidValue.Instance);

					return node;
			}
		}

		private ISetting SettingCreate ()
		{
			CustomSetting setting;

			setting = new CustomSetting ();
			setting.BlockBegin = this.parameters.BlockBegin;
			setting.BlockContinue = this.parameters.BlockContinue;
			setting.BlockEnd = this.parameters.BlockEnd;
			setting.Trimmer = TrimmerCollection.GetTrimmer (this.parameters.TrimmerIndex);

			return setting;
		}

		private void StateLoad (string path, bool dialog)
		{
			TreeNode root;
			Dictionary<string, Value> values;
			int version;

			if (this.treeViewValue.Nodes.Count < 1)
				return;

			root = this.treeViewValue.Nodes[0];
			root.Nodes.Clear ();

			try
			{
				using (BinaryReader reader = new BinaryReader (new FileStream (path, FileMode.Open), Encoding.UTF8))
				{
					values = new Dictionary<string, Value> ();
					version = reader.ReadInt32 ();

					if (version < 1 || version > 2)
					{
						MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "Incompatible file format"));

						return;
					}

					if (ValueAccessor.Load (reader, values))
					{
						foreach (KeyValuePair<string, Value> pair in values)
							root.Nodes.Add (this.NodeCreate (pair.Key, pair.Value));
					}

					this.parameters = new SettingForm.Parameters
					{
						BlockBegin = reader.ReadString (),
						BlockContinue = reader.ReadString (),
						BlockEnd = reader.ReadString (),
						TrimmerIndex = version > 1 ? reader.ReadInt32 () : TrimmerCollection.DefaultIndex
					};

					this.textBoxInput.Text = reader.ReadString ();
				}

				root.ExpandAll ();

				if (dialog)
					MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "State successfully loaded from \"{0}\".", path), "File save successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch
			{
				MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "Cannot open input file \"{0}\"", path), "File load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void StateSave (string path)
		{
			Dictionary<string, Value> values = new Dictionary<string, Value> ();

			foreach (TreeNode root in this.treeViewValue.Nodes)
			{
				foreach (KeyValuePair<Value, Value> pair in this.ValuesBuild (root.Nodes))
					values[pair.Key.AsString] = pair.Value;
			}

			try
			{
				using (BinaryWriter writer = new BinaryWriter (new FileStream (path, FileMode.Create), Encoding.UTF8))
				{
					writer.Write (2);

					ValueAccessor.Save (writer, values);

					writer.Write (this.parameters.BlockBegin);
					writer.Write (this.parameters.BlockContinue);
					writer.Write (this.parameters.BlockEnd);
					writer.Write (this.parameters.TrimmerIndex);
					writer.Write (this.textBoxInput.Text);
				}

				MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "State successfully saved to \"{0}\".", path), "File save successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch
			{
				MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "Cannot open output file \"{0}\"", path), "File save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private List<KeyValuePair<Value, Value>> ValuesBuild (TreeNodeCollection nodes)
		{
			List<KeyValuePair<Value, Value>> collection = new List<KeyValuePair<Value,Value>> (nodes.Count);
			NodeData data;

			foreach (TreeNode node in nodes)
			{
				data = node.Tag as NodeData;

				if (data != null)
				{
					switch (data.Value.Type)
					{
						case ValueContent.Map:
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

#endregion
	}
}