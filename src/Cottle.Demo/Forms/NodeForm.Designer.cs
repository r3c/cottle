using System.ComponentModel;
using System.Windows.Forms;

namespace Cottle.Demo.Forms
{
	partial class NodeForm
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
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.checkBoxValueBoolean = new System.Windows.Forms.CheckBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.textBoxValueString = new System.Windows.Forms.TextBox();
			this.textBoxValueNumber = new System.Windows.Forms.TextBox();
			this.radioButtonValueUndefined = new System.Windows.Forms.RadioButton();
			this.radioButtonValueString = new System.Windows.Forms.RadioButton();
			this.radioButtonValueNumber = new System.Windows.Forms.RadioButton();
			this.radioButtonValueBoolean = new System.Windows.Forms.RadioButton();
			this.radioButtonValueMap = new System.Windows.Forms.RadioButton();
			this.labelContent = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.Location = new System.Drawing.Point(117, 24);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(400, 23);
			this.textBoxName.TabIndex = 1;
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.checkBoxValueBoolean);
			this.groupBox.Controls.Add(this.buttonCancel);
			this.groupBox.Controls.Add(this.textBoxValueString);
			this.groupBox.Controls.Add(this.textBoxValueNumber);
			this.groupBox.Controls.Add(this.radioButtonValueUndefined);
			this.groupBox.Controls.Add(this.radioButtonValueString);
			this.groupBox.Controls.Add(this.radioButtonValueNumber);
			this.groupBox.Controls.Add(this.radioButtonValueBoolean);
			this.groupBox.Controls.Add(this.radioButtonValueMap);
			this.groupBox.Controls.Add(this.labelContent);
			this.groupBox.Controls.Add(this.labelName);
			this.groupBox.Controls.Add(this.buttonAccept);
			this.groupBox.Controls.Add(this.textBoxName);
			this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox.Location = new System.Drawing.Point(6, 6);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(529, 240);
			this.groupBox.TabIndex = 2;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Value parameters:";
			// 
			// checkBoxValueBoolean
			// 
			this.checkBoxValueBoolean.AutoSize = true;
			this.checkBoxValueBoolean.Location = new System.Drawing.Point(233, 87);
			this.checkBoxValueBoolean.Name = "checkBoxValueBoolean";
			this.checkBoxValueBoolean.Size = new System.Drawing.Size(58, 19);
			this.checkBoxValueBoolean.TabIndex = 6;
			this.checkBoxValueBoolean.Text = "Is true";
			this.checkBoxValueBoolean.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Image = global::Cottle.Demo.Resources.Glyph.button_cancel;
			this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonCancel.Location = new System.Drawing.Point(267, 202);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 30);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// textBoxValueString
			// 
			this.textBoxValueString.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxValueString.Location = new System.Drawing.Point(233, 145);
			this.textBoxValueString.Name = "textBoxValueString";
			this.textBoxValueString.Size = new System.Drawing.Size(283, 23);
			this.textBoxValueString.TabIndex = 10;
			// 
			// textBoxValueNumber
			// 
			this.textBoxValueNumber.Anchor =
				((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
				                                       System.Windows.Forms.AnchorStyles.Left) |
				                                      System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxValueNumber.Location = new System.Drawing.Point(233, 115);
			this.textBoxValueNumber.Name = "textBoxValueNumber";
			this.textBoxValueNumber.Size = new System.Drawing.Size(283, 23);
			this.textBoxValueNumber.TabIndex = 8;
			// 
			// radioButtonValueUndefined
			// 
			this.radioButtonValueUndefined.AutoSize = true;
			this.radioButtonValueUndefined.Checked = true;
			this.radioButtonValueUndefined.Location = new System.Drawing.Point(117, 177);
			this.radioButtonValueUndefined.Name = "radioButtonValueUndefined";
			this.radioButtonValueUndefined.Size = new System.Drawing.Size(111, 19);
			this.radioButtonValueUndefined.TabIndex = 11;
			this.radioButtonValueUndefined.TabStop = true;
			this.radioButtonValueUndefined.Text = "Undefined value";
			this.radioButtonValueUndefined.UseVisualStyleBackColor = true;
			this.radioButtonValueUndefined.CheckedChanged +=
				new System.EventHandler(this.radioButtonValue_CheckedChanged);
			// 
			// radioButtonValueString
			// 
			this.radioButtonValueString.AutoSize = true;
			this.radioButtonValueString.Location = new System.Drawing.Point(117, 147);
			this.radioButtonValueString.Name = "radioButtonValueString";
			this.radioButtonValueString.Size = new System.Drawing.Size(87, 19);
			this.radioButtonValueString.TabIndex = 9;
			this.radioButtonValueString.Text = "String value";
			this.radioButtonValueString.UseVisualStyleBackColor = true;
			this.radioButtonValueString.CheckedChanged += new System.EventHandler(this.radioButtonValue_CheckedChanged);
			// 
			// radioButtonValueNumber
			// 
			this.radioButtonValueNumber.AutoSize = true;
			this.radioButtonValueNumber.Location = new System.Drawing.Point(117, 117);
			this.radioButtonValueNumber.Name = "radioButtonValueNumber";
			this.radioButtonValueNumber.Size = new System.Drawing.Size(100, 19);
			this.radioButtonValueNumber.TabIndex = 7;
			this.radioButtonValueNumber.Text = "Number value";
			this.radioButtonValueNumber.UseVisualStyleBackColor = true;
			this.radioButtonValueNumber.CheckedChanged += new System.EventHandler(this.radioButtonValue_CheckedChanged);
			// 
			// radioButtonValueBoolean
			// 
			this.radioButtonValueBoolean.AutoSize = true;
			this.radioButtonValueBoolean.Location = new System.Drawing.Point(117, 87);
			this.radioButtonValueBoolean.Name = "radioButtonValueBoolean";
			this.radioButtonValueBoolean.Size = new System.Drawing.Size(99, 19);
			this.radioButtonValueBoolean.TabIndex = 5;
			this.radioButtonValueBoolean.TabStop = true;
			this.radioButtonValueBoolean.Text = "Boolean value";
			this.radioButtonValueBoolean.UseVisualStyleBackColor = true;
			this.radioButtonValueBoolean.CheckedChanged +=
				new System.EventHandler(this.radioButtonValue_CheckedChanged);
			// 
			// radioButtonValueMap
			// 
			this.radioButtonValueMap.AutoSize = true;
			this.radioButtonValueMap.Location = new System.Drawing.Point(117, 57);
			this.radioButtonValueMap.Name = "radioButtonValueMap";
			this.radioButtonValueMap.Size = new System.Drawing.Size(80, 19);
			this.radioButtonValueMap.TabIndex = 4;
			this.radioButtonValueMap.TabStop = true;
			this.radioButtonValueMap.Text = "Map value";
			this.radioButtonValueMap.UseVisualStyleBackColor = true;
			this.radioButtonValueMap.CheckedChanged += new System.EventHandler(this.radioButtonValue_CheckedChanged);
			// 
			// labelContent
			// 
			this.labelContent.AutoSize = true;
			this.labelContent.Location = new System.Drawing.Point(9, 58);
			this.labelContent.Name = "labelContent";
			this.labelContent.Size = new System.Drawing.Size(82, 15);
			this.labelContent.TabIndex = 3;
			this.labelContent.Text = "Variable value:";
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(9, 28);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(84, 15);
			this.labelName.TabIndex = 2;
			this.labelName.Text = "Variable name:";
			// 
			// buttonAccept
			// 
			this.buttonAccept.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonAccept.Image = global::Cottle.Demo.Resources.Glyph.button_accept;
			this.buttonAccept.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAccept.Location = new System.Drawing.Point(144, 202);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(117, 30);
			this.buttonAccept.TabIndex = 12;
			this.buttonAccept.Text = "Insert";
			this.buttonAccept.UseVisualStyleBackColor = true;
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// NodeForm
			// 
			this.AcceptButton = this.buttonAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(541, 252);
			this.Controls.Add(this.groupBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NodeForm";
			this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Variable declaration";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxValueBoolean;
		private System.Windows.Forms.TextBox textBoxValueNumber;
		private System.Windows.Forms.TextBox textBoxValueString;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton radioButtonValueMap;
		private System.Windows.Forms.RadioButton radioButtonValueBoolean;
		private System.Windows.Forms.RadioButton radioButtonValueNumber;
		private System.Windows.Forms.RadioButton radioButtonValueString;
		private System.Windows.Forms.RadioButton radioButtonValueUndefined;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelContent;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonAccept;
	}
}