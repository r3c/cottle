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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof (DemoForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer ();
            this.splitContainerInput = new System.Windows.Forms.SplitContainer ();
            this.groupBoxValue = new System.Windows.Forms.GroupBox ();
            this.treeViewValue = new System.Windows.Forms.TreeView ();
            this.groupBoxResult = new System.Windows.Forms.GroupBox ();
            this.buttonDemo = new System.Windows.Forms.Button ();
            this.textBoxResult = new System.Windows.Forms.TextBox ();
            this.groupBoxInput = new System.Windows.Forms.GroupBox ();
            this.textBoxInput = new System.Windows.Forms.TextBox ();
            this.groupBoxOutput = new System.Windows.Forms.GroupBox ();
            this.textBoxPrint = new System.Windows.Forms.TextBox ();
            this.splitContainer.Panel1.SuspendLayout ();
            this.splitContainer.Panel2.SuspendLayout ();
            this.splitContainer.SuspendLayout ();
            this.splitContainerInput.Panel1.SuspendLayout ();
            this.splitContainerInput.Panel2.SuspendLayout ();
            this.splitContainerInput.SuspendLayout ();
            this.groupBoxValue.SuspendLayout ();
            this.groupBoxResult.SuspendLayout ();
            this.groupBoxInput.SuspendLayout ();
            this.groupBoxOutput.SuspendLayout ();
            this.SuspendLayout ();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point (5, 5);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add (this.splitContainerInput);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add (this.groupBoxOutput);
            this.splitContainer.Size = new System.Drawing.Size (774, 555);
            this.splitContainer.SplitterDistance = 385;
            this.splitContainer.TabIndex = 1;
            // 
            // splitContainerInput
            // 
            this.splitContainerInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerInput.Location = new System.Drawing.Point (0, 0);
            this.splitContainerInput.Name = "splitContainerInput";
            this.splitContainerInput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerInput.Panel1
            // 
            this.splitContainerInput.Panel1.Controls.Add (this.groupBoxValue);
            // 
            // splitContainerInput.Panel2
            // 
            this.splitContainerInput.Panel2.Controls.Add (this.groupBoxResult);
            this.splitContainerInput.Panel2.Controls.Add (this.groupBoxInput);
            this.splitContainerInput.Size = new System.Drawing.Size (385, 555);
            this.splitContainerInput.SplitterDistance = 150;
            this.splitContainerInput.TabIndex = 0;
            // 
            // groupBoxValue
            // 
            this.groupBoxValue.Controls.Add (this.treeViewValue);
            this.groupBoxValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxValue.Location = new System.Drawing.Point (0, 0);
            this.groupBoxValue.Name = "groupBoxValue";
            this.groupBoxValue.Padding = new System.Windows.Forms.Padding (7);
            this.groupBoxValue.Size = new System.Drawing.Size (385, 150);
            this.groupBoxValue.TabIndex = 5;
            this.groupBoxValue.TabStop = false;
            this.groupBoxValue.Text = "Input values:";
            // 
            // treeViewValue
            // 
            this.treeViewValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewValue.Location = new System.Drawing.Point (7, 20);
            this.treeViewValue.Name = "treeViewValue";
            this.treeViewValue.Size = new System.Drawing.Size (371, 123);
            this.treeViewValue.TabIndex = 0;
            // 
            // groupBoxResult
            // 
            this.groupBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxResult.Controls.Add (this.buttonDemo);
            this.groupBoxResult.Controls.Add (this.textBoxResult);
            this.groupBoxResult.Location = new System.Drawing.Point (0, 353);
            this.groupBoxResult.Name = "groupBoxResult";
            this.groupBoxResult.Size = new System.Drawing.Size (385, 48);
            this.groupBoxResult.TabIndex = 4;
            this.groupBoxResult.TabStop = false;
            this.groupBoxResult.Text = "Parse && print:";
            // 
            // buttonDemo
            // 
            this.buttonDemo.Location = new System.Drawing.Point (6, 19);
            this.buttonDemo.Name = "buttonDemo";
            this.buttonDemo.Size = new System.Drawing.Size (60, 23);
            this.buttonDemo.TabIndex = 1;
            this.buttonDemo.Text = "OK";
            this.buttonDemo.UseVisualStyleBackColor = true;
            this.buttonDemo.Click += new System.EventHandler (this.buttonDemo_Click);
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.Location = new System.Drawing.Point (72, 21);
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.Size = new System.Drawing.Size (306, 20);
            this.textBoxResult.TabIndex = 2;
            // 
            // groupBoxInput
            // 
            this.groupBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInput.Controls.Add (this.textBoxInput);
            this.groupBoxInput.Location = new System.Drawing.Point (0, 0);
            this.groupBoxInput.Name = "groupBoxInput";
            this.groupBoxInput.Padding = new System.Windows.Forms.Padding (7);
            this.groupBoxInput.Size = new System.Drawing.Size (385, 347);
            this.groupBoxInput.TabIndex = 3;
            this.groupBoxInput.TabStop = false;
            this.groupBoxInput.Text = "Input template:";
            // 
            // textBoxInput
            // 
            this.textBoxInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInput.Font = new System.Drawing.Font ("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInput.Location = new System.Drawing.Point (7, 20);
            this.textBoxInput.Multiline = true;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxInput.Size = new System.Drawing.Size (371, 320);
            this.textBoxInput.TabIndex = 0;
            this.textBoxInput.Text = resources.GetString ("textBoxInput.Text");
            this.textBoxInput.WordWrap = false;
            // 
            // groupBoxOutput
            // 
            this.groupBoxOutput.Controls.Add (this.textBoxPrint);
            this.groupBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOutput.Location = new System.Drawing.Point (0, 0);
            this.groupBoxOutput.Name = "groupBoxOutput";
            this.groupBoxOutput.Padding = new System.Windows.Forms.Padding (7);
            this.groupBoxOutput.Size = new System.Drawing.Size (385, 555);
            this.groupBoxOutput.TabIndex = 4;
            this.groupBoxOutput.TabStop = false;
            this.groupBoxOutput.Text = "Output:";
            // 
            // textBoxPrint
            // 
            this.textBoxPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPrint.Font = new System.Drawing.Font ("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPrint.Location = new System.Drawing.Point (7, 20);
            this.textBoxPrint.Multiline = true;
            this.textBoxPrint.Name = "textBoxPrint";
            this.textBoxPrint.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPrint.Size = new System.Drawing.Size (371, 528);
            this.textBoxPrint.TabIndex = 3;
            this.textBoxPrint.WordWrap = false;
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size (784, 565);
            this.Controls.Add (this.splitContainer);
            this.Name = "DemoForm";
            this.Padding = new System.Windows.Forms.Padding (5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cottle Demo";
            this.splitContainer.Panel1.ResumeLayout (false);
            this.splitContainer.Panel2.ResumeLayout (false);
            this.splitContainer.ResumeLayout (false);
            this.splitContainerInput.Panel1.ResumeLayout (false);
            this.splitContainerInput.Panel2.ResumeLayout (false);
            this.splitContainerInput.ResumeLayout (false);
            this.groupBoxValue.ResumeLayout (false);
            this.groupBoxResult.ResumeLayout (false);
            this.groupBoxResult.PerformLayout ();
            this.groupBoxInput.ResumeLayout (false);
            this.groupBoxInput.PerformLayout ();
            this.groupBoxOutput.ResumeLayout (false);
            this.groupBoxOutput.PerformLayout ();
            this.ResumeLayout (false);

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
    }
}