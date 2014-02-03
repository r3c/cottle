using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Cottle;
using Cottle.Commons;
using Cottle.Documents;
using Cottle.Exceptions;
using Cottle.Scopes;
using Cottle.Settings;
using Cottle.Values;

namespace	Demo
{
	public partial class	DemoForm : Form
	{
		#region Constants

		private const string	AUTOLOAD = "autoload.ctv";

		#endregion

		#region Attributes / Instance

		private ISetting	setting = DefaultSetting.Instance;

		#endregion

		#region Constructors

		public	DemoForm ()
		{
			InitializeComponent ();

			this.treeViewValue.Nodes.Add (new TreeNode ("Cottle Scope", 6, 6));

			if (File.Exists (DemoForm.AUTOLOAD))
				this.StateLoad (DemoForm.AUTOLOAD, false);
		}

		#endregion

		#region Methods / Listeners

		private void	buttonClean_Click (object sender, EventArgs e)
		{
			IDocument	document;

			try
			{
				document = new SimpleDocument (this.textBoxInput.Text, this.setting);

				this.textBoxInput.Text = document.Source ();
			}
			catch (UnexpectedException exception)
			{
				this.textBoxInput.SelectionStart = Math.Max (exception.Index - exception.Lexem.Length - 1, 0);
				this.textBoxInput.SelectionLength = exception.Lexem.Length;

				throw;
			}
			catch (UnknownException exception)
			{
				this.textBoxInput.SelectionStart = Math.Max (exception.Index - 1, 0);
				this.textBoxInput.SelectionLength = 1;

				throw;
			}
		}

		private void	buttonEvaluate_Click (object sender, EventArgs e)
		{
			IDocument	document;
			IScope		scope;

			try
			{
				try
				{
					document = new SimpleDocument (this.textBoxInput.Text, this.setting);
					scope = new DefaultScope ();

					foreach (TreeNode root in this.treeViewValue.Nodes)
					{
						foreach (KeyValuePair<Value, Value> pair in this.ValuesBuild (root.Nodes))
							scope.Set (pair.Key, pair.Value, ScopeMode.Closest);
					}

					this.textBoxPrint.BackColor = Color.LightGreen;
					this.textBoxPrint.Text = document.Render (scope);
				}
				catch (UnexpectedException exception)
				{
					this.textBoxInput.SelectionStart = Math.Max (exception.Index - exception.Lexem.Length - 1, 0);
					this.textBoxInput.SelectionLength = exception.Lexem.Length;

					throw;
				}
				catch (UnknownException exception)
				{
					this.textBoxInput.SelectionStart = Math.Max (exception.Index - 1, 0);
					this.textBoxInput.SelectionLength = 1;

					throw;
				}
			}
			catch (DocumentException exception)
			{
				this.textBoxInput.Focus ();
				this.textBoxPrint.BackColor = Color.LightPink;
				this.textBoxPrint.Text = "Document error: " + exception.Message;
			}
			catch (RenderException exception)
			{
				this.textBoxPrint.BackColor = Color.LightSalmon;
				this.textBoxPrint.Text = "Render error: " + exception.Message;
			}
		}

		private void	toolStripMenuItemSetting_Click (object sender, EventArgs e)
		{
			Form	form;

			form = new SettingForm (this.setting, (setting) => this.setting = setting);
			form.ShowDialog (this);
		}

		private void	toolStripMenuItemFileLoad_Click (object sender, EventArgs e)
		{
			OpenFileDialog	dialog = new OpenFileDialog ();

			dialog.Filter = "Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*";

			if (dialog.ShowDialog (this) == DialogResult.OK)
				this.StateLoad (dialog.FileName, true);
		}

		private void	toolStripMenuItemFileSave_Click (object sender, EventArgs e)
		{
			SaveFileDialog	dialog = new SaveFileDialog ();

			dialog.Filter = "Cottle values file (*.ctv)|*.ctv|Any file (*.*)|*.*";

			if (dialog.ShowDialog (this) == DialogResult.OK)
				this.StateSave (dialog.FileName);
		}

		private void	toolStripMenuItemMoveDown_Click (object sender, EventArgs e)
		{
			TreeNodeCollection	collection;
			int					index1;
			int					index2;
			TreeNode			node1 = this.contextMenuStripTree.Tag as TreeNode;
			TreeNode			node2;

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

		private void	toolStripMenuItemMoveUp_Click (object sender, EventArgs e)
		{
			TreeNodeCollection	collection;
			int					index1;
			int					index2;
			TreeNode			node1 = this.contextMenuStripTree.Tag as TreeNode;
			TreeNode			node2;

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

		private void	toolStripMenuItemNodeClone_Click (object sender, EventArgs e)
		{
			TreeNode	node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null && node.Parent != null)
				node.Parent.Nodes.Insert (node.Index + 1, this.NodeClone (node));
		}

		private void	toolStripMenuItemNodeCreate_Click (object sender, EventArgs e)
		{
			Form		form;
			TreeNode	node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null)
			{
				form = new NodeForm (null, delegate (string key, Value value)
				{
					TreeNode	child = this.NodeCreate (key, value);

					node.Nodes.Add (child);
					node.Expand ();
				});

				form.ShowDialog (this);
			}
		}

		private void	toolStripMenuItemNodeDelete_Click (object sender, EventArgs e)
		{
			TreeNode	node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null && node.Parent != null)
				node.Remove ();
		}

		private void	toolStripMenuItemNodeUpdate_Click (object sender, EventArgs e)
		{
			Form		form;
			TreeNode	node = this.contextMenuStripTree.Tag as TreeNode;

			if (node != null)
			{
				form = new NodeForm (node.Tag as NodeData, (key, value) => this.NodeAssign (node, key, value));
				form.ShowDialog (this);
			}
		}

		private void	toolStripMenuItemTreeCollapse_Click (object sender, EventArgs e)
		{
			this.treeViewValue.CollapseAll ();
		}

		private void	toolStripMenuItemTreeExpand_Click (object sender, EventArgs e)
		{
			this.treeViewValue.ExpandAll ();
		}

		private void	contextMenuStripTree_Opening (object sender, CancelEventArgs e)
		{
			TreeNode	node = this.treeViewValue.SelectedNode;
			NodeData	data = node != null ? node.Tag as NodeData : null;

			this.toolStripMenuItemNodeClone.Enabled = node != null && node.Parent != null;
			this.toolStripMenuItemNodeCreate.Enabled = node != null && node.Parent == null || (data != null && data.Value.Type == ValueContent.Map);
			this.toolStripMenuItemNodeDelete.Enabled = node != null && node.Parent != null;
			this.toolStripMenuItemMoveDown.Enabled = node != null && node.Parent != null && node != null && node.NextNode != null;
			this.toolStripMenuItemMoveUp.Enabled = node != null && node.Parent != null && node != null && node.PrevNode != null;
			this.toolStripMenuItemNodeUpdate.Enabled = node != null && node.Parent != null;

			this.contextMenuStripTree.Tag = node;
		}

		#endregion

		#region Methods / Private

		private void	NodeAssign (TreeNode node, string key, Value value)
		{
			NodeData	data = new NodeData (key, value);

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

		private TreeNode	NodeClone (TreeNode node)
		{
			NodeData	data = node.Tag as NodeData;
			TreeNode	copy;

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

		private TreeNode	NodeCreate (string key, Value value)
		{
			TreeNode	node = new TreeNode ();
			TreeNode[]	range;
			int			i;

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

					this.NodeAssign (node, key, new MapValue ());

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

		private void	StateLoad (string path, bool dialog)
		{
			TreeNode					root;
			CustomSetting				setting;
			Dictionary<string, Value>   values;
			int						 	version;

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

					if (CommonTools.ValuesLoad (reader, values))
					{
						foreach (KeyValuePair<string, Value> pair in values)
							root.Nodes.Add (this.NodeCreate (pair.Key, pair.Value));
					}

					setting = new CustomSetting ();
					setting.BlockBegin = reader.ReadString ();
					setting.BlockContinue = reader.ReadString ();
					setting.BlockEnd = reader.ReadString ();
					setting.Cleaner = CleanerCollection.GetCleaner (version > 1 ? reader.ReadInt32 () : -1);

					this.setting = setting;
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

		private void	StateSave (string path)
		{
			Dictionary<string, Value>	values = new Dictionary<string, Value> ();

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

					CommonTools.ValuesSave (writer, values);

					writer.Write (this.setting.BlockBegin);
					writer.Write (this.setting.BlockContinue);
					writer.Write (this.setting.BlockEnd);
					writer.Write (CleanerCollection.GetIndex (this.setting.Cleaner));
					writer.Write (this.textBoxInput.Text);
				}

				MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "State successfully saved to \"{0}\".", path), "File save successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch
			{
				MessageBox.Show (this, string.Format (CultureInfo.InvariantCulture, "Cannot open output file \"{0}\"", path), "File save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private List<KeyValuePair<Value, Value>>	ValuesBuild (TreeNodeCollection nodes)
		{
			List<KeyValuePair<Value, Value>>	collection = new List<KeyValuePair<Value,Value>> (nodes.Count);
			NodeData							data;

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
