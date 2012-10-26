namespace Demo
{
    partial class DemoForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent ()
        {
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoForm));
        	this.splitContainer = new System.Windows.Forms.SplitContainer();
        	this.splitContainerInput = new System.Windows.Forms.SplitContainer();
        	this.groupBoxValue = new System.Windows.Forms.GroupBox();
        	this.treeViewValue = new System.Windows.Forms.TreeView();
        	this.contextMenuStripTree = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.toolStripMenuItemNodeCreate = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        	this.toolStripMenuItemNodeClone = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemNodeUpdate = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemNodeDelete = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemMoveUp = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemMoveDown = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        	this.toolStripMenuItemFileLoad = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemConfig = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        	this.toolStripMenuItemTreeCollapse = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItemTreeExpand = new System.Windows.Forms.ToolStripMenuItem();
        	this.imageListTree = new System.Windows.Forms.ImageList(this.components);
        	this.groupBoxResult = new System.Windows.Forms.GroupBox();
        	this.buttonDemo = new System.Windows.Forms.Button();
        	this.textBoxResult = new System.Windows.Forms.TextBox();
        	this.groupBoxInput = new System.Windows.Forms.GroupBox();
        	this.textBoxInput = new System.Windows.Forms.TextBox();
        	this.groupBoxOutput = new System.Windows.Forms.GroupBox();
        	this.textBoxPrint = new System.Windows.Forms.TextBox();
        	this.splitContainer.Panel1.SuspendLayout();
        	this.splitContainer.Panel2.SuspendLayout();
        	this.splitContainer.SuspendLayout();
        	this.splitContainerInput.Panel1.SuspendLayout();
        	this.splitContainerInput.Panel2.SuspendLayout();
        	this.splitContainerInput.SuspendLayout();
        	this.groupBoxValue.SuspendLayout();
        	this.contextMenuStripTree.SuspendLayout();
        	this.groupBoxResult.SuspendLayout();
        	this.groupBoxInput.SuspendLayout();
        	this.groupBoxOutput.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// splitContainer
        	// 
        	this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainer.Location = new System.Drawing.Point(5, 5);
        	this.splitContainer.Name = "splitContainer";
        	// 
        	// splitContainer.Panel1
        	// 
        	this.splitContainer.Panel1.Controls.Add(this.splitContainerInput);
        	// 
        	// splitContainer.Panel2
        	// 
        	this.splitContainer.Panel2.Controls.Add(this.groupBoxOutput);
        	this.splitContainer.Size = new System.Drawing.Size(774, 555);
        	this.splitContainer.SplitterDistance = 385;
        	this.splitContainer.TabIndex = 1;
        	// 
        	// splitContainerInput
        	// 
        	this.splitContainerInput.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.splitContainerInput.Location = new System.Drawing.Point(0, 0);
        	this.splitContainerInput.Name = "splitContainerInput";
        	this.splitContainerInput.Orientation = System.Windows.Forms.Orientation.Horizontal;
        	// 
        	// splitContainerInput.Panel1
        	// 
        	this.splitContainerInput.Panel1.Controls.Add(this.groupBoxValue);
        	// 
        	// splitContainerInput.Panel2
        	// 
        	this.splitContainerInput.Panel2.Controls.Add(this.groupBoxResult);
        	this.splitContainerInput.Panel2.Controls.Add(this.groupBoxInput);
        	this.splitContainerInput.Size = new System.Drawing.Size(385, 555);
        	this.splitContainerInput.SplitterDistance = 150;
        	this.splitContainerInput.TabIndex = 0;
        	// 
        	// groupBoxValue
        	// 
        	this.groupBoxValue.Controls.Add(this.treeViewValue);
        	this.groupBoxValue.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.groupBoxValue.Location = new System.Drawing.Point(0, 0);
        	this.groupBoxValue.Name = "groupBoxValue";
        	this.groupBoxValue.Padding = new System.Windows.Forms.Padding(7);
        	this.groupBoxValue.Size = new System.Drawing.Size(385, 150);
        	this.groupBoxValue.TabIndex = 5;
        	this.groupBoxValue.TabStop = false;
        	this.groupBoxValue.Text = "Input values:";
        	// 
        	// treeViewValue
        	// 
        	this.treeViewValue.ContextMenuStrip = this.contextMenuStripTree;
        	this.treeViewValue.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.treeViewValue.ImageIndex = 0;
        	this.treeViewValue.ImageList = this.imageListTree;
        	this.treeViewValue.Location = new System.Drawing.Point(7, 20);
        	this.treeViewValue.Name = "treeViewValue";
        	this.treeViewValue.SelectedImageIndex = 0;
        	this.treeViewValue.Size = new System.Drawing.Size(371, 123);
        	this.treeViewValue.TabIndex = 0;
        	// 
        	// contextMenuStripTree
        	// 
        	this.contextMenuStripTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.toolStripMenuItemNodeCreate,
        	        	        	this.toolStripSeparator1,
        	        	        	this.toolStripMenuItemNodeClone,
        	        	        	this.toolStripMenuItemNodeUpdate,
        	        	        	this.toolStripMenuItemNodeDelete,
        	        	        	this.toolStripMenuItemMoveUp,
        	        	        	this.toolStripMenuItemMoveDown,
        	        	        	this.toolStripSeparator2,
        	        	        	this.toolStripMenuItemFileLoad,
        	        	        	this.toolStripMenuItemFileSave,
        	        	        	this.toolStripMenuItemConfig,
        	        	        	this.toolStripSeparator3,
        	        	        	this.toolStripMenuItemTreeCollapse,
        	        	        	this.toolStripMenuItemTreeExpand});
        	this.contextMenuStripTree.Name = "contextMenuStripTree";
        	this.contextMenuStripTree.Size = new System.Drawing.Size(161, 286);
        	this.contextMenuStripTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTree_Opening);
        	// 
        	// toolStripMenuItemNodeCreate
        	// 
        	this.toolStripMenuItemNodeCreate.Image = global::Demo.Properties.Resources.menu_node_create;
        	this.toolStripMenuItemNodeCreate.Name = "toolStripMenuItemNodeCreate";
        	this.toolStripMenuItemNodeCreate.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemNodeCreate.Text = "Insert value here";
        	this.toolStripMenuItemNodeCreate.Click += new System.EventHandler(this.toolStripMenuItemNodeCreate_Click);
        	// 
        	// toolStripSeparator1
        	// 
        	this.toolStripSeparator1.Name = "toolStripSeparator1";
        	this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
        	// 
        	// toolStripMenuItemNodeClone
        	// 
        	this.toolStripMenuItemNodeClone.Image = global::Demo.Properties.Resources.menu_node_clone;
        	this.toolStripMenuItemNodeClone.Name = "toolStripMenuItemNodeClone";
        	this.toolStripMenuItemNodeClone.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemNodeClone.Text = "Clone value";
        	this.toolStripMenuItemNodeClone.Click += new System.EventHandler(this.toolStripMenuItemNodeClone_Click);
        	// 
        	// toolStripMenuItemNodeUpdate
        	// 
        	this.toolStripMenuItemNodeUpdate.Image = global::Demo.Properties.Resources.menu_node_update;
        	this.toolStripMenuItemNodeUpdate.Name = "toolStripMenuItemNodeUpdate";
        	this.toolStripMenuItemNodeUpdate.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemNodeUpdate.Text = "Modify value";
        	this.toolStripMenuItemNodeUpdate.Click += new System.EventHandler(this.toolStripMenuItemNodeUpdate_Click);
        	// 
        	// toolStripMenuItemNodeDelete
        	// 
        	this.toolStripMenuItemNodeDelete.Image = global::Demo.Properties.Resources.menu_node_delete;
        	this.toolStripMenuItemNodeDelete.Name = "toolStripMenuItemNodeDelete";
        	this.toolStripMenuItemNodeDelete.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemNodeDelete.Text = "Delete value";
        	this.toolStripMenuItemNodeDelete.Click += new System.EventHandler(this.toolStripMenuItemNodeDelete_Click);
        	// 
        	// toolStripMenuItemMoveUp
        	// 
        	this.toolStripMenuItemMoveUp.Image = global::Demo.Properties.Resources.menu_move_up;
        	this.toolStripMenuItemMoveUp.Name = "toolStripMenuItemMoveUp";
        	this.toolStripMenuItemMoveUp.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemMoveUp.Text = "Move up";
        	this.toolStripMenuItemMoveUp.Click += new System.EventHandler(this.toolStripMenuItemMoveUp_Click);
        	// 
        	// toolStripMenuItemMoveDown
        	// 
        	this.toolStripMenuItemMoveDown.Image = global::Demo.Properties.Resources.menu_move_down;
        	this.toolStripMenuItemMoveDown.Name = "toolStripMenuItemMoveDown";
        	this.toolStripMenuItemMoveDown.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemMoveDown.Text = "Move down";
        	this.toolStripMenuItemMoveDown.Click += new System.EventHandler(this.toolStripMenuItemMoveDown_Click);
        	// 
        	// toolStripSeparator2
        	// 
        	this.toolStripSeparator2.Name = "toolStripSeparator2";
        	this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
        	// 
        	// toolStripMenuItemFileLoad
        	// 
        	this.toolStripMenuItemFileLoad.Image = global::Demo.Properties.Resources.menu_file_load;
        	this.toolStripMenuItemFileLoad.Name = "toolStripMenuItemFileLoad";
        	this.toolStripMenuItemFileLoad.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemFileLoad.Text = "Load state...";
        	this.toolStripMenuItemFileLoad.Click += new System.EventHandler(this.toolStripMenuItemFileLoad_Click);
        	// 
        	// toolStripMenuItemFileSave
        	// 
        	this.toolStripMenuItemFileSave.Image = global::Demo.Properties.Resources.menu_file_save;
        	this.toolStripMenuItemFileSave.Name = "toolStripMenuItemFileSave";
        	this.toolStripMenuItemFileSave.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemFileSave.Text = "Save state...";
        	this.toolStripMenuItemFileSave.Click += new System.EventHandler(this.toolStripMenuItemFileSave_Click);
        	// 
        	// toolStripMenuItemConfig
        	// 
        	this.toolStripMenuItemConfig.Image = global::Demo.Properties.Resources.menu_config;
        	this.toolStripMenuItemConfig.Name = "toolStripMenuItemConfig";
        	this.toolStripMenuItemConfig.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemConfig.Text = "Configuration...";
        	this.toolStripMenuItemConfig.Click += new System.EventHandler(this.toolStripMenuItemConfig_Click);
        	// 
        	// toolStripSeparator3
        	// 
        	this.toolStripSeparator3.Name = "toolStripSeparator3";
        	this.toolStripSeparator3.Size = new System.Drawing.Size(157, 6);
        	// 
        	// toolStripMenuItemTreeCollapse
        	// 
        	this.toolStripMenuItemTreeCollapse.Image = global::Demo.Properties.Resources.menu_tree_collapse;
        	this.toolStripMenuItemTreeCollapse.Name = "toolStripMenuItemTreeCollapse";
        	this.toolStripMenuItemTreeCollapse.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemTreeCollapse.Text = "Collapse all";
        	this.toolStripMenuItemTreeCollapse.Click += new System.EventHandler(this.toolStripMenuItemTreeCollapse_Click);
        	// 
        	// toolStripMenuItemTreeExpand
        	// 
        	this.toolStripMenuItemTreeExpand.Image = global::Demo.Properties.Resources.menu_tree_expand;
        	this.toolStripMenuItemTreeExpand.Name = "toolStripMenuItemTreeExpand";
        	this.toolStripMenuItemTreeExpand.Size = new System.Drawing.Size(160, 22);
        	this.toolStripMenuItemTreeExpand.Text = "Expand all";
        	this.toolStripMenuItemTreeExpand.Click += new System.EventHandler(this.toolStripMenuItemTreeExpand_Click);
        	// 
        	// imageListTree
        	// 
        	this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
        	this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
        	this.imageListTree.Images.SetKeyName(0, "value_array.png");
        	this.imageListTree.Images.SetKeyName(1, "value_boolean.png");
        	this.imageListTree.Images.SetKeyName(2, "value_function.png");
        	this.imageListTree.Images.SetKeyName(3, "value_number.png");
        	this.imageListTree.Images.SetKeyName(4, "value_string.png");
        	this.imageListTree.Images.SetKeyName(5, "value_undefined.png");
        	this.imageListTree.Images.SetKeyName(6, "scope.png");
        	// 
        	// groupBoxResult
        	// 
        	this.groupBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBoxResult.Controls.Add(this.buttonDemo);
        	this.groupBoxResult.Controls.Add(this.textBoxResult);
        	this.groupBoxResult.Location = new System.Drawing.Point(0, 353);
        	this.groupBoxResult.Name = "groupBoxResult";
        	this.groupBoxResult.Size = new System.Drawing.Size(385, 48);
        	this.groupBoxResult.TabIndex = 4;
        	this.groupBoxResult.TabStop = false;
        	this.groupBoxResult.Text = "Validate && debug:";
        	// 
        	// buttonDemo
        	// 
        	this.buttonDemo.Image = global::Demo.Properties.Resources.button_submit;
        	this.buttonDemo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        	this.buttonDemo.Location = new System.Drawing.Point(6, 19);
        	this.buttonDemo.Name = "buttonDemo";
        	this.buttonDemo.Size = new System.Drawing.Size(80, 23);
        	this.buttonDemo.TabIndex = 1;
        	this.buttonDemo.Text = "OK";
        	this.buttonDemo.UseVisualStyleBackColor = true;
        	this.buttonDemo.Click += new System.EventHandler(this.buttonDemo_Click);
        	// 
        	// textBoxResult
        	// 
        	this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.textBoxResult.Location = new System.Drawing.Point(92, 21);
        	this.textBoxResult.Name = "textBoxResult";
        	this.textBoxResult.Size = new System.Drawing.Size(286, 20);
        	this.textBoxResult.TabIndex = 2;
        	// 
        	// groupBoxInput
        	// 
        	this.groupBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBoxInput.Controls.Add(this.textBoxInput);
        	this.groupBoxInput.Location = new System.Drawing.Point(0, 0);
        	this.groupBoxInput.Name = "groupBoxInput";
        	this.groupBoxInput.Padding = new System.Windows.Forms.Padding(7);
        	this.groupBoxInput.Size = new System.Drawing.Size(385, 347);
        	this.groupBoxInput.TabIndex = 3;
        	this.groupBoxInput.TabStop = false;
        	this.groupBoxInput.Text = "Input template:";
        	// 
        	// textBoxInput
        	// 
        	this.textBoxInput.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.textBoxInput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.textBoxInput.Location = new System.Drawing.Point(7, 20);
        	this.textBoxInput.Multiline = true;
        	this.textBoxInput.Name = "textBoxInput";
        	this.textBoxInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        	this.textBoxInput.Size = new System.Drawing.Size(371, 320);
        	this.textBoxInput.TabIndex = 0;
        	this.textBoxInput.WordWrap = false;
        	// 
        	// groupBoxOutput
        	// 
        	this.groupBoxOutput.Controls.Add(this.textBoxPrint);
        	this.groupBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.groupBoxOutput.Location = new System.Drawing.Point(0, 0);
        	this.groupBoxOutput.Name = "groupBoxOutput";
        	this.groupBoxOutput.Padding = new System.Windows.Forms.Padding(7);
        	this.groupBoxOutput.Size = new System.Drawing.Size(385, 555);
        	this.groupBoxOutput.TabIndex = 4;
        	this.groupBoxOutput.TabStop = false;
        	this.groupBoxOutput.Text = "Output:";
        	// 
        	// textBoxPrint
        	// 
        	this.textBoxPrint.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.textBoxPrint.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.textBoxPrint.Location = new System.Drawing.Point(7, 20);
        	this.textBoxPrint.Multiline = true;
        	this.textBoxPrint.Name = "textBoxPrint";
        	this.textBoxPrint.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        	this.textBoxPrint.Size = new System.Drawing.Size(371, 528);
        	this.textBoxPrint.TabIndex = 3;
        	this.textBoxPrint.WordWrap = false;
        	// 
        	// DemoForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(784, 565);
        	this.Controls.Add(this.splitContainer);
        	this.Name = "DemoForm";
        	this.Padding = new System.Windows.Forms.Padding(5);
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        	this.Text = "Cottle Demo";
        	this.splitContainer.Panel1.ResumeLayout(false);
        	this.splitContainer.Panel2.ResumeLayout(false);
        	this.splitContainer.ResumeLayout(false);
        	this.splitContainerInput.Panel1.ResumeLayout(false);
        	this.splitContainerInput.Panel2.ResumeLayout(false);
        	this.splitContainerInput.ResumeLayout(false);
        	this.groupBoxValue.ResumeLayout(false);
        	this.contextMenuStripTree.ResumeLayout(false);
        	this.groupBoxResult.ResumeLayout(false);
        	this.groupBoxResult.PerformLayout();
        	this.groupBoxInput.ResumeLayout(false);
        	this.groupBoxInput.PerformLayout();
        	this.groupBoxOutput.ResumeLayout(false);
        	this.groupBoxOutput.PerformLayout();
        	this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button buttonDemo;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.TextBox textBoxPrint;
        private System.Windows.Forms.GroupBox groupBoxOutput;
        private System.Windows.Forms.GroupBox groupBoxResult;
        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.SplitContainer splitContainerInput;
        private System.Windows.Forms.GroupBox groupBoxValue;
        private System.Windows.Forms.TreeView treeViewValue;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTree;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeCreate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMoveUp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMoveDown;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileLoad;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFileSave;
        private System.Windows.Forms.ImageList imageListTree;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeUpdate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNodeClone;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTreeCollapse;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTreeExpand;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemConfig;
    }
}