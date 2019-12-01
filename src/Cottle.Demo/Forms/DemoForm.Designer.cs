using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Cottle.Demo.Properties;

namespace Cottle.Demo.Forms
{
	partial class DemoForm
	{
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private IContainer components = null;

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Code généré par le Concepteur Windows Form

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(typeof(Cottle.Demo.Forms.DemoForm));
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainerInput = new System.Windows.Forms.SplitContainer();
			this.groupBoxContext = new System.Windows.Forms.GroupBox();
			this.treeViewContext = new System.Windows.Forms.TreeView();
			this.contextMenuStripTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemNodeCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemNodeClone = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemNodeUpdate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemNodeDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemMoveUp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemMoveDown = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemTreeCollapse = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemTreeExpand = new System.Windows.Forms.ToolStripMenuItem();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.groupBoxTemplate = new System.Windows.Forms.GroupBox();
			this.textBoxTemplate = new System.Windows.Forms.TextBox();
			this.groupBoxControl = new System.Windows.Forms.GroupBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.buttonConfiguration = new System.Windows.Forms.Button();
			this.buttonEvaluate = new System.Windows.Forms.Button();
			this.groupBoxOutput = new System.Windows.Forms.GroupBox();
			this.textBoxOutput = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerInput)).BeginInit();
			this.splitContainerInput.Panel1.SuspendLayout();
			this.splitContainerInput.Panel2.SuspendLayout();
			this.splitContainerInput.SuspendLayout();
			this.groupBoxContext.SuspendLayout();
			this.contextMenuStripTree.SuspendLayout();
			this.groupBoxTemplate.SuspendLayout();
			this.groupBoxControl.SuspendLayout();
			this.groupBoxOutput.SuspendLayout();
			this.SuspendLayout();
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(6, 6);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Panel1.Controls.Add(this.splitContainerInput);
			this.splitContainer.Panel2.Controls.Add(this.groupBoxOutput);
			this.splitContainer.Size = new System.Drawing.Size(996, 717);
			this.splitContainer.SplitterDistance = 495;
			this.splitContainer.SplitterWidth = 5;
			this.splitContainer.TabIndex = 1;
			this.splitContainerInput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerInput.Location = new System.Drawing.Point(0, 0);
			this.splitContainerInput.Name = "splitContainerInput";
			this.splitContainerInput.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainerInput.Panel1.Controls.Add(this.groupBoxContext);
			this.splitContainerInput.Panel2.Controls.Add(this.groupBoxTemplate);
			this.splitContainerInput.Panel2.Controls.Add(this.groupBoxControl);
			this.splitContainerInput.Size = new System.Drawing.Size(495, 717);
			this.splitContainerInput.SplitterDistance = 192;
			this.splitContainerInput.SplitterWidth = 5;
			this.splitContainerInput.TabIndex = 0;
			this.groupBoxContext.Controls.Add(this.treeViewContext);
			this.groupBoxContext.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxContext.Location = new System.Drawing.Point(0, 0);
			this.groupBoxContext.Name = "groupBoxContext";
			this.groupBoxContext.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
			this.groupBoxContext.Size = new System.Drawing.Size(495, 192);
			this.groupBoxContext.TabIndex = 5;
			this.groupBoxContext.TabStop = false;
			this.groupBoxContext.Text = "Context variables:";
			this.treeViewContext.ContextMenuStrip = this.contextMenuStripTree;
			this.treeViewContext.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewContext.ImageIndex = 0;
			this.treeViewContext.ImageList = this.imageListTree;
			this.treeViewContext.Location = new System.Drawing.Point(8, 24);
			this.treeViewContext.Name = "treeViewContext";
			this.treeViewContext.SelectedImageIndex = 0;
			this.treeViewContext.Size = new System.Drawing.Size(479, 160);
			this.treeViewContext.TabIndex = 0;
			this.contextMenuStripTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
			{
				this.toolStripMenuItemNodeCreate, this.toolStripSeparator1, this.toolStripMenuItemNodeClone,
				this.toolStripMenuItemNodeUpdate, this.toolStripMenuItemNodeDelete, this.toolStripMenuItemMoveUp,
				this.toolStripMenuItemMoveDown, this.toolStripSeparator2, this.toolStripMenuItemTreeCollapse,
				this.toolStripMenuItemTreeExpand
			});
			this.contextMenuStripTree.Name = "contextMenuStripTree";
			this.contextMenuStripTree.Size = new System.Drawing.Size(161, 192);
			this.contextMenuStripTree.Opening +=
				new System.ComponentModel.CancelEventHandler(this.contextMenuStripTree_Opening);
			this.toolStripMenuItemNodeCreate.Image = global::Cottle.Demo.Resources.Glyph.menu_node_create;
			this.toolStripMenuItemNodeCreate.Name = "toolStripMenuItemNodeCreate";
			this.toolStripMenuItemNodeCreate.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemNodeCreate.Text = "Insert value here";
			this.toolStripMenuItemNodeCreate.Click += new System.EventHandler(this.toolStripMenuItemNodeCreate_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
			this.toolStripMenuItemNodeClone.Image = global::Cottle.Demo.Resources.Glyph.menu_node_clone;
			this.toolStripMenuItemNodeClone.Name = "toolStripMenuItemNodeClone";
			this.toolStripMenuItemNodeClone.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemNodeClone.Text = "Clone value";
			this.toolStripMenuItemNodeClone.Click += new System.EventHandler(this.toolStripMenuItemNodeClone_Click);
			this.toolStripMenuItemNodeUpdate.Image = global::Cottle.Demo.Resources.Glyph.menu_node_update;
			this.toolStripMenuItemNodeUpdate.Name = "toolStripMenuItemNodeUpdate";
			this.toolStripMenuItemNodeUpdate.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemNodeUpdate.Text = "Modify value";
			this.toolStripMenuItemNodeUpdate.Click += new System.EventHandler(this.toolStripMenuItemNodeUpdate_Click);
			this.toolStripMenuItemNodeDelete.Image = global::Cottle.Demo.Resources.Glyph.menu_node_delete;
			this.toolStripMenuItemNodeDelete.Name = "toolStripMenuItemNodeDelete";
			this.toolStripMenuItemNodeDelete.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemNodeDelete.Text = "Delete value";
			this.toolStripMenuItemNodeDelete.Click += new System.EventHandler(this.toolStripMenuItemNodeDelete_Click);
			this.toolStripMenuItemMoveUp.Image = global::Cottle.Demo.Resources.Glyph.menu_move_up;
			this.toolStripMenuItemMoveUp.Name = "toolStripMenuItemMoveUp";
			this.toolStripMenuItemMoveUp.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemMoveUp.Text = "Move up";
			this.toolStripMenuItemMoveUp.Click += new System.EventHandler(this.toolStripMenuItemMoveUp_Click);
			this.toolStripMenuItemMoveDown.Image = global::Cottle.Demo.Resources.Glyph.menu_move_down;
			this.toolStripMenuItemMoveDown.Name = "toolStripMenuItemMoveDown";
			this.toolStripMenuItemMoveDown.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemMoveDown.Text = "Move down";
			this.toolStripMenuItemMoveDown.Click += new System.EventHandler(this.toolStripMenuItemMoveDown_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
			this.toolStripMenuItemTreeCollapse.Image = global::Cottle.Demo.Resources.Glyph.menu_tree_collapse;
			this.toolStripMenuItemTreeCollapse.Name = "toolStripMenuItemTreeCollapse";
			this.toolStripMenuItemTreeCollapse.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemTreeCollapse.Text = "Collapse all";
			this.toolStripMenuItemTreeCollapse.Click +=
				new System.EventHandler(this.toolStripMenuItemTreeCollapse_Click);
			this.toolStripMenuItemTreeExpand.Image = global::Cottle.Demo.Resources.Glyph.menu_tree_expand;
			this.toolStripMenuItemTreeExpand.Name = "toolStripMenuItemTreeExpand";
			this.toolStripMenuItemTreeExpand.Size = new System.Drawing.Size(160, 22);
			this.toolStripMenuItemTreeExpand.Text = "Expand all";
			this.toolStripMenuItemTreeExpand.Click += new System.EventHandler(this.toolStripMenuItemTreeExpand_Click);
			this.imageListTree.ImageStream =
				((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "value_array.png");
			this.imageListTree.Images.SetKeyName(1, "value_boolean.png");
			this.imageListTree.Images.SetKeyName(2, "value_function.png");
			this.imageListTree.Images.SetKeyName(3, "value_number.png");
			this.imageListTree.Images.SetKeyName(4, "value_string.png");
			this.imageListTree.Images.SetKeyName(5, "value_undefined.png");
			this.imageListTree.Images.SetKeyName(6, "scope.png");
			this.groupBoxTemplate.Controls.Add(this.textBoxTemplate);
			this.groupBoxTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxTemplate.Location = new System.Drawing.Point(0, 0);
			this.groupBoxTemplate.Name = "groupBoxTemplate";
			this.groupBoxTemplate.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
			this.groupBoxTemplate.Size = new System.Drawing.Size(495, 462);
			this.groupBoxTemplate.TabIndex = 3;
			this.groupBoxTemplate.TabStop = false;
			this.groupBoxTemplate.Text = "Document template:";
			this.textBoxTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxTemplate.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular,
				System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxTemplate.Location = new System.Drawing.Point(8, 24);
			this.textBoxTemplate.Multiline = true;
			this.textBoxTemplate.Name = "textBoxTemplate";
			this.textBoxTemplate.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxTemplate.Size = new System.Drawing.Size(479, 430);
			this.textBoxTemplate.TabIndex = 0;
			this.groupBoxControl.Controls.Add(this.buttonSave);
			this.groupBoxControl.Controls.Add(this.buttonLoad);
			this.groupBoxControl.Controls.Add(this.buttonConfiguration);
			this.groupBoxControl.Controls.Add(this.buttonEvaluate);
			this.groupBoxControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBoxControl.Location = new System.Drawing.Point(0, 462);
			this.groupBoxControl.Name = "groupBoxControl";
			this.groupBoxControl.Size = new System.Drawing.Size(495, 58);
			this.groupBoxControl.TabIndex = 4;
			this.groupBoxControl.TabStop = false;
			this.groupBoxControl.Text = "Control:";
			this.buttonSave.Image = global::Cottle.Demo.Resources.Glyph.menu_file_save;
			this.buttonSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonSave.Location = new System.Drawing.Point(355, 22);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(110, 30);
			this.buttonSave.TabIndex = 6;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			this.buttonLoad.Image = global::Cottle.Demo.Resources.Glyph.menu_file_load;
			this.buttonLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonLoad.Location = new System.Drawing.Point(239, 22);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(110, 30);
			this.buttonLoad.TabIndex = 5;
			this.buttonLoad.Text = "Load";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			this.buttonConfiguration.Image = global::Cottle.Demo.Resources.Glyph.menu_config;
			this.buttonConfiguration.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonConfiguration.Location = new System.Drawing.Point(122, 22);
			this.buttonConfiguration.Name = "buttonConfiguration";
			this.buttonConfiguration.Size = new System.Drawing.Size(110, 30);
			this.buttonConfiguration.TabIndex = 4;
			this.buttonConfiguration.Text = "Configure";
			this.buttonConfiguration.UseVisualStyleBackColor = true;
			this.buttonConfiguration.Click += new System.EventHandler(this.buttonConfiguration_Click);
			this.buttonEvaluate.Image = global::Cottle.Demo.Resources.Glyph.menu_evaluate;
			this.buttonEvaluate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonEvaluate.Location = new System.Drawing.Point(7, 22);
			this.buttonEvaluate.Name = "buttonEvaluate";
			this.buttonEvaluate.Size = new System.Drawing.Size(110, 30);
			this.buttonEvaluate.TabIndex = 1;
			this.buttonEvaluate.Text = "Evaluate";
			this.buttonEvaluate.UseVisualStyleBackColor = true;
			this.buttonEvaluate.Click += new System.EventHandler(this.buttonEvaluate_Click);
			this.groupBoxOutput.Controls.Add(this.textBoxOutput);
			this.groupBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxOutput.Location = new System.Drawing.Point(0, 0);
			this.groupBoxOutput.Name = "groupBoxOutput";
			this.groupBoxOutput.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
			this.groupBoxOutput.Size = new System.Drawing.Size(496, 717);
			this.groupBoxOutput.TabIndex = 4;
			this.groupBoxOutput.TabStop = false;
			this.groupBoxOutput.Text = "Output:";
			this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular,
				System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxOutput.Location = new System.Drawing.Point(8, 24);
			this.textBoxOutput.Multiline = true;
			this.textBoxOutput.Name = "textBoxOutput";
			this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxOutput.Size = new System.Drawing.Size(480, 685);
			this.textBoxOutput.TabIndex = 3;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 729);
			this.Controls.Add(this.splitContainer);
			this.Name = "DemoForm";
			this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cottle Demo";
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.splitContainerInput.Panel1.ResumeLayout(false);
			this.splitContainerInput.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerInput)).EndInit();
			this.splitContainerInput.ResumeLayout(false);
			this.groupBoxContext.ResumeLayout(false);
			this.contextMenuStripTree.ResumeLayout(false);
			this.groupBoxTemplate.ResumeLayout(false);
			this.groupBoxTemplate.PerformLayout();
			this.groupBoxControl.ResumeLayout(false);
			this.groupBoxOutput.ResumeLayout(false);
			this.groupBoxOutput.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxContext;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTreeExpand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTreeCollapse;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeClone;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeUpdate;
		private System.Windows.Forms.ImageList imageListTree;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeDelete;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMoveDown;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMoveUp;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeCreate;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripTree;
		private System.Windows.Forms.SplitContainer splitContainerInput;
		private System.Windows.Forms.GroupBox groupBoxOutput;
		private System.Windows.Forms.TextBox textBoxOutput;
		private System.Windows.Forms.Button buttonEvaluate;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.TextBox textBoxTemplate;
		private System.Windows.Forms.GroupBox groupBoxControl;
		private System.Windows.Forms.GroupBox groupBoxTemplate;
		private System.Windows.Forms.TreeView treeViewContext;
		private System.Windows.Forms.Button buttonConfiguration;
		private System.Windows.Forms.Button buttonLoad;
		private System.Windows.Forms.Button buttonSave;
	}
}