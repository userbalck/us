namespace Nopedemo2
{
    partial class fmFind
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
            this.bfind = new System.Windows.Forms.Button();
            this.bcancel = new System.Windows.Forms.Button();
            this.tfindBox = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoDown = new System.Windows.Forms.RadioButton();
            this.rdoUP = new System.Windows.Forms.RadioButton();
            this.chkUpper = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bfind
            // 
            this.bfind.Location = new System.Drawing.Point(382, 26);
            this.bfind.Name = "bfind";
            this.bfind.Size = new System.Drawing.Size(108, 23);
            this.bfind.TabIndex = 0;
            this.bfind.Text = "查找下一个";
            this.bfind.UseVisualStyleBackColor = true;
            // 
            // bcancel
            // 
            this.bcancel.Location = new System.Drawing.Point(382, 71);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(106, 23);
            this.bcancel.TabIndex = 1;
            this.bcancel.Text = "取消";
            this.bcancel.UseVisualStyleBackColor = true;
            // 
            // tfindBox
            // 
            this.tfindBox.Location = new System.Drawing.Point(129, 28);
            this.tfindBox.Name = "tfindBox";
            this.tfindBox.Size = new System.Drawing.Size(234, 21);
            this.tfindBox.TabIndex = 2;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.Location = new System.Drawing.Point(22, 31);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(77, 14);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "查找内容：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoDown);
            this.groupBox1.Controls.Add(this.rdoUP);
            this.groupBox1.Location = new System.Drawing.Point(129, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 54);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "方向";
            // 
            // rdoDown
            // 
            this.rdoDown.AutoSize = true;
            this.rdoDown.Location = new System.Drawing.Point(65, 23);
            this.rdoDown.Name = "rdoDown";
            this.rdoDown.Size = new System.Drawing.Size(47, 16);
            this.rdoDown.TabIndex = 1;
            this.rdoDown.TabStop = true;
            this.rdoDown.Text = "向下";
            this.rdoDown.UseVisualStyleBackColor = true;
            // 
            // rdoUP
            // 
            this.rdoUP.AutoSize = true;
            this.rdoUP.Location = new System.Drawing.Point(0, 23);
            this.rdoUP.Name = "rdoUP";
            this.rdoUP.Size = new System.Drawing.Size(47, 16);
            this.rdoUP.TabIndex = 0;
            this.rdoUP.TabStop = true;
            this.rdoUP.Text = "向上";
            this.rdoUP.UseVisualStyleBackColor = true;
            // 
            // chkUpper
            // 
            this.chkUpper.AutoSize = true;
            this.chkUpper.Location = new System.Drawing.Point(24, 79);
            this.chkUpper.Name = "chkUpper";
            this.chkUpper.Size = new System.Drawing.Size(84, 16);
            this.chkUpper.TabIndex = 5;
            this.chkUpper.Text = "区分大小写";
            this.chkUpper.UseVisualStyleBackColor = true;
            // 
            // fmFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 148);
            this.Controls.Add(this.chkUpper);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.tfindBox);
            this.Controls.Add(this.bcancel);
            this.Controls.Add(this.bfind);
            this.Name = "fmFind";
            this.Text = "查找";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bfind;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.TextBox tfindBox;
        private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdoDown;
		private System.Windows.Forms.RadioButton rdoUP;
		private System.Windows.Forms.CheckBox chkUpper;
	}
}