namespace Demo
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.textBoxBlockContinue = new System.Windows.Forms.TextBox();
            this.labelBlockContinue = new System.Windows.Forms.Label();
            this.textBoxBlockEnd = new System.Windows.Forms.TextBox();
            this.labelBlockEnd = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBlockBegin = new System.Windows.Forms.Label();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.textBoxBlockBegin = new System.Windows.Forms.TextBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.textBoxBlockContinue);
            this.groupBox.Controls.Add(this.labelBlockContinue);
            this.groupBox.Controls.Add(this.textBoxBlockEnd);
            this.groupBox.Controls.Add(this.labelBlockEnd);
            this.groupBox.Controls.Add(this.buttonCancel);
            this.groupBox.Controls.Add(this.labelBlockBegin);
            this.groupBox.Controls.Add(this.buttonAccept);
            this.groupBox.Controls.Add(this.textBoxBlockBegin);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(5, 5);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(274, 132);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Library configuration:";
            // 
            // textBoxBlockContinue
            // 
            this.textBoxBlockContinue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBlockContinue.Location = new System.Drawing.Point(120, 47);
            this.textBoxBlockContinue.Name = "textBoxBlockContinue";
            this.textBoxBlockContinue.Size = new System.Drawing.Size(144, 20);
            this.textBoxBlockContinue.TabIndex = 1;
            // 
            // labelBlockContinue
            // 
            this.labelBlockContinue.AutoSize = true;
            this.labelBlockContinue.Location = new System.Drawing.Point(8, 50);
            this.labelBlockContinue.Name = "labelBlockContinue";
            this.labelBlockContinue.Size = new System.Drawing.Size(99, 13);
            this.labelBlockContinue.TabIndex = 16;
            this.labelBlockContinue.Text = "Block continue tag:";
            // 
            // textBoxBlockEnd
            // 
            this.textBoxBlockEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBlockEnd.Location = new System.Drawing.Point(120, 73);
            this.textBoxBlockEnd.Name = "textBoxBlockEnd";
            this.textBoxBlockEnd.Size = new System.Drawing.Size(144, 20);
            this.textBoxBlockEnd.TabIndex = 2;
            // 
            // labelBlockEnd
            // 
            this.labelBlockEnd.AutoSize = true;
            this.labelBlockEnd.Location = new System.Drawing.Point(8, 76);
            this.labelBlockEnd.Name = "labelBlockEnd";
            this.labelBlockEnd.Size = new System.Drawing.Size(76, 13);
            this.labelBlockEnd.TabIndex = 14;
            this.labelBlockEnd.Text = "Block end tag:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::Demo.Properties.Resources.cross;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.Location = new System.Drawing.Point(140, 102);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelBlockBegin
            // 
            this.labelBlockBegin.AutoSize = true;
            this.labelBlockBegin.Location = new System.Drawing.Point(8, 24);
            this.labelBlockBegin.Name = "labelBlockBegin";
            this.labelBlockBegin.Size = new System.Drawing.Size(84, 13);
            this.labelBlockBegin.TabIndex = 2;
            this.labelBlockBegin.Text = "Block begin tag:";
            // 
            // buttonAccept
            // 
            this.buttonAccept.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAccept.Image = global::Demo.Properties.Resources.tick;
            this.buttonAccept.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAccept.Location = new System.Drawing.Point(34, 102);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(100, 23);
            this.buttonAccept.TabIndex = 3;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // textBoxBlockBegin
            // 
            this.textBoxBlockBegin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBlockBegin.Location = new System.Drawing.Point(120, 21);
            this.textBoxBlockBegin.Name = "textBoxBlockBegin";
            this.textBoxBlockBegin.Size = new System.Drawing.Size(144, 20);
            this.textBoxBlockBegin.TabIndex = 0;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(284, 142);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Library configuration";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelBlockBegin;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.TextBox textBoxBlockBegin;
        private System.Windows.Forms.Label labelBlockEnd;
        private System.Windows.Forms.TextBox textBoxBlockEnd;
        private System.Windows.Forms.TextBox textBoxBlockContinue;
        private System.Windows.Forms.Label labelBlockContinue;
    }
}