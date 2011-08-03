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
            this.groupBoxResult = new System.Windows.Forms.GroupBox ();
            this.buttonDemo = new System.Windows.Forms.Button ();
            this.textBoxResult = new System.Windows.Forms.TextBox ();
            this.groupBoxInput = new System.Windows.Forms.GroupBox ();
            this.textBoxInput = new System.Windows.Forms.TextBox ();
            this.splitContainerOutput = new System.Windows.Forms.SplitContainer ();
            this.groupBoxDebug = new System.Windows.Forms.GroupBox ();
            this.textBoxDebug = new System.Windows.Forms.TextBox ();
            this.groupBoxPrint = new System.Windows.Forms.GroupBox ();
            this.textBoxPrint = new System.Windows.Forms.TextBox ();
            this.splitContainer.Panel1.SuspendLayout ();
            this.splitContainer.Panel2.SuspendLayout ();
            this.splitContainer.SuspendLayout ();
            this.groupBoxResult.SuspendLayout ();
            this.groupBoxInput.SuspendLayout ();
            this.splitContainerOutput.Panel1.SuspendLayout ();
            this.splitContainerOutput.Panel2.SuspendLayout ();
            this.splitContainerOutput.SuspendLayout ();
            this.groupBoxDebug.SuspendLayout ();
            this.groupBoxPrint.SuspendLayout ();
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
            this.splitContainer.Panel1.Controls.Add (this.groupBoxResult);
            this.splitContainer.Panel1.Controls.Add (this.groupBoxInput);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add (this.splitContainerOutput);
            this.splitContainer.Size = new System.Drawing.Size (774, 555);
            this.splitContainer.SplitterDistance = 385;
            this.splitContainer.TabIndex = 1;
            // 
            // groupBoxResult
            // 
            this.groupBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxResult.Controls.Add (this.buttonDemo);
            this.groupBoxResult.Controls.Add (this.textBoxResult);
            this.groupBoxResult.Location = new System.Drawing.Point (0, 507);
            this.groupBoxResult.Name = "groupBoxResult";
            this.groupBoxResult.Size = new System.Drawing.Size (385, 48);
            this.groupBoxResult.TabIndex = 4;
            this.groupBoxResult.TabStop = false;
            this.groupBoxResult.Text = "Parse && evaluate:";
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
            this.groupBoxInput.Size = new System.Drawing.Size (385, 501);
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
            this.textBoxInput.Size = new System.Drawing.Size (371, 474);
            this.textBoxInput.TabIndex = 0;
            this.textBoxInput.Text = resources.GetString ("textBoxInput.Text");
            // 
            // splitContainerOutput
            // 
            this.splitContainerOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerOutput.Location = new System.Drawing.Point (0, 0);
            this.splitContainerOutput.Name = "splitContainerOutput";
            this.splitContainerOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerOutput.Panel1
            // 
            this.splitContainerOutput.Panel1.Controls.Add (this.groupBoxDebug);
            // 
            // splitContainerOutput.Panel2
            // 
            this.splitContainerOutput.Panel2.Controls.Add (this.groupBoxPrint);
            this.splitContainerOutput.Size = new System.Drawing.Size (385, 555);
            this.splitContainerOutput.SplitterDistance = 277;
            this.splitContainerOutput.TabIndex = 4;
            // 
            // groupBoxDebug
            // 
            this.groupBoxDebug.Controls.Add (this.textBoxDebug);
            this.groupBoxDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDebug.Location = new System.Drawing.Point (0, 0);
            this.groupBoxDebug.Name = "groupBoxDebug";
            this.groupBoxDebug.Padding = new System.Windows.Forms.Padding (7);
            this.groupBoxDebug.Size = new System.Drawing.Size (385, 277);
            this.groupBoxDebug.TabIndex = 2;
            this.groupBoxDebug.TabStop = false;
            this.groupBoxDebug.Text = "Debug output:";
            // 
            // textBoxDebug
            // 
            this.textBoxDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDebug.Font = new System.Drawing.Font ("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDebug.Location = new System.Drawing.Point (7, 20);
            this.textBoxDebug.Multiline = true;
            this.textBoxDebug.Name = "textBoxDebug";
            this.textBoxDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDebug.Size = new System.Drawing.Size (371, 250);
            this.textBoxDebug.TabIndex = 1;
            // 
            // groupBoxPrint
            // 
            this.groupBoxPrint.Controls.Add (this.textBoxPrint);
            this.groupBoxPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPrint.Location = new System.Drawing.Point (0, 0);
            this.groupBoxPrint.Name = "groupBoxPrint";
            this.groupBoxPrint.Padding = new System.Windows.Forms.Padding (7);
            this.groupBoxPrint.Size = new System.Drawing.Size (385, 274);
            this.groupBoxPrint.TabIndex = 4;
            this.groupBoxPrint.TabStop = false;
            this.groupBoxPrint.Text = "Evaluation output:";
            // 
            // textBoxPrint
            // 
            this.textBoxPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPrint.Font = new System.Drawing.Font ("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPrint.Location = new System.Drawing.Point (7, 20);
            this.textBoxPrint.Multiline = true;
            this.textBoxPrint.Name = "textBoxPrint";
            this.textBoxPrint.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPrint.Size = new System.Drawing.Size (371, 247);
            this.textBoxPrint.TabIndex = 3;
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
            this.groupBoxResult.ResumeLayout (false);
            this.groupBoxResult.PerformLayout ();
            this.groupBoxInput.ResumeLayout (false);
            this.groupBoxInput.PerformLayout ();
            this.splitContainerOutput.Panel1.ResumeLayout (false);
            this.splitContainerOutput.Panel2.ResumeLayout (false);
            this.splitContainerOutput.ResumeLayout (false);
            this.groupBoxDebug.ResumeLayout (false);
            this.groupBoxDebug.PerformLayout ();
            this.groupBoxPrint.ResumeLayout (false);
            this.groupBoxPrint.PerformLayout ();
            this.ResumeLayout (false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.TextBox textBoxDebug;
        private System.Windows.Forms.Button buttonDemo;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.TextBox textBoxPrint;
        private System.Windows.Forms.SplitContainer splitContainerOutput;
        private System.Windows.Forms.GroupBox groupBoxDebug;
        private System.Windows.Forms.GroupBox groupBoxPrint;
        private System.Windows.Forms.GroupBox groupBoxResult;
        private System.Windows.Forms.GroupBox groupBoxInput;
    }
}