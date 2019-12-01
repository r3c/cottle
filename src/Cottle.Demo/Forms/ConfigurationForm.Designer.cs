using System.ComponentModel;
using System.Windows.Forms;

namespace Cottle.Demo.Forms
{
	partial class ConfigurationForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
			this.comboBoxTrimmer = new System.Windows.Forms.ComboBox();
			this.labelTrimmer = new System.Windows.Forms.Label();
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
			this.groupBox.Controls.Add(this.comboBoxTrimmer);
			this.groupBox.Controls.Add(this.labelTrimmer);
			this.groupBox.Controls.Add(this.textBoxBlockContinue);
			this.groupBox.Controls.Add(this.labelBlockContinue);
			this.groupBox.Controls.Add(this.textBoxBlockEnd);
			this.groupBox.Controls.Add(this.labelBlockEnd);
			this.groupBox.Controls.Add(this.buttonCancel);
			this.groupBox.Controls.Add(this.labelBlockBegin);
			this.groupBox.Controls.Add(this.buttonAccept);
			this.groupBox.Controls.Add(this.textBoxBlockBegin);
			this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox.Location = new System.Drawing.Point(6, 6);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(392, 181);
			this.groupBox.TabIndex = 3;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Library configuration:";
			// 
			// comboBoxTrimmer
			// 
			this.comboBoxTrimmer.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxTrimmer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTrimmer.FormattingEnabled = true;
			this.comboBoxTrimmer.Location = new System.Drawing.Point(140, 114);
			this.comboBoxTrimmer.Name = "comboBoxTrimmer";
			this.comboBoxTrimmer.Size = new System.Drawing.Size(238, 23);
			this.comboBoxTrimmer.TabIndex = 3;
			// 
			// labelTrimmer
			// 
			this.labelTrimmer.AutoSize = true;
			this.labelTrimmer.Location = new System.Drawing.Point(9, 118);
			this.labelTrimmer.Name = "labelTrimmer";
			this.labelTrimmer.Size = new System.Drawing.Size(95, 15);
			this.labelTrimmer.TabIndex = 17;
			this.labelTrimmer.Text = "Trimming mode:";
			// 
			// textBoxBlockContinue
			// 
			this.textBoxBlockContinue.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxBlockContinue.Location = new System.Drawing.Point(140, 54);
			this.textBoxBlockContinue.Name = "textBoxBlockContinue";
			this.textBoxBlockContinue.Size = new System.Drawing.Size(238, 23);
			this.textBoxBlockContinue.TabIndex = 1;
			// 
			// labelBlockContinue
			// 
			this.labelBlockContinue.AutoSize = true;
			this.labelBlockContinue.Location = new System.Drawing.Point(9, 58);
			this.labelBlockContinue.Name = "labelBlockContinue";
			this.labelBlockContinue.Size = new System.Drawing.Size(109, 15);
			this.labelBlockContinue.TabIndex = 16;
			this.labelBlockContinue.Text = "Block continue tag:";
			// 
			// textBoxBlockEnd
			// 
			this.textBoxBlockEnd.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxBlockEnd.Location = new System.Drawing.Point(140, 84);
			this.textBoxBlockEnd.Name = "textBoxBlockEnd";
			this.textBoxBlockEnd.Size = new System.Drawing.Size(238, 23);
			this.textBoxBlockEnd.TabIndex = 2;
			// 
			// labelBlockEnd
			// 
			this.labelBlockEnd.AutoSize = true;
			this.labelBlockEnd.Location = new System.Drawing.Point(9, 88);
			this.labelBlockEnd.Name = "labelBlockEnd";
			this.labelBlockEnd.Size = new System.Drawing.Size(82, 15);
			this.labelBlockEnd.TabIndex = 14;
			this.labelBlockEnd.Text = "Block end tag:";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Image = global::Cottle.Demo.Resources.Glyph.button_cancel;
			this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonCancel.Location = new System.Drawing.Point(199, 147);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 30);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelBlockBegin
			// 
			this.labelBlockBegin.AutoSize = true;
			this.labelBlockBegin.Location = new System.Drawing.Point(9, 28);
			this.labelBlockBegin.Name = "labelBlockBegin";
			this.labelBlockBegin.Size = new System.Drawing.Size(92, 15);
			this.labelBlockBegin.TabIndex = 2;
			this.labelBlockBegin.Text = "Block begin tag:";
			// 
			// buttonAccept
			// 
			this.buttonAccept.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonAccept.Image = global::Cottle.Demo.Resources.Glyph.button_accept;
			this.buttonAccept.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAccept.Location = new System.Drawing.Point(76, 147);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(117, 30);
			this.buttonAccept.TabIndex = 4;
			this.buttonAccept.Text = "Accept";
			this.buttonAccept.UseVisualStyleBackColor = true;
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// textBoxBlockBegin
			// 
			this.textBoxBlockBegin.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxBlockBegin.Location = new System.Drawing.Point(140, 24);
			this.textBoxBlockBegin.Name = "textBoxBlockBegin";
			this.textBoxBlockBegin.Size = new System.Drawing.Size(238, 23);
			this.textBoxBlockBegin.TabIndex = 0;
			// 
			// ConfigurationForm
			// 
			this.AcceptButton = this.buttonAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(404, 193);
			this.Controls.Add(this.groupBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigurationForm";
			this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Document configuration";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);
		}

		private System.Windows.Forms.Label labelTrimmer;
		private System.Windows.Forms.ComboBox comboBoxTrimmer;

		#endregion

		private System.Windows.Forms.Label labelBlockContinue;
		private System.Windows.Forms.TextBox textBoxBlockContinue;
		private System.Windows.Forms.TextBox textBoxBlockEnd;
		private System.Windows.Forms.Label labelBlockEnd;
		private System.Windows.Forms.TextBox textBoxBlockBegin;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Label labelBlockBegin;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox;
	}
}