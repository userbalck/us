namespace Nopedemo2
{
	partial class Noperepl
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
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.tb_content = new System.Windows.Forms.TextBox();
			this.tb_Repl = new System.Windows.Forms.TextBox();
			this.btfind2 = new System.Windows.Forms.Button();
			this.bt_Repl = new System.Windows.Forms.Button();
			this.bt_Whole = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.cb_Size = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(36, 26);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(65, 12);
			this.linkLabel1.TabIndex = 0;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "查找内容：";
			// 
			// linkLabel2
			// 
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Location = new System.Drawing.Point(36, 57);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(53, 12);
			this.linkLabel2.TabIndex = 1;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "替换为：";
			// 
			// tb_content
			// 
			this.tb_content.Location = new System.Drawing.Point(108, 17);
			this.tb_content.Name = "tb_content";
			this.tb_content.Size = new System.Drawing.Size(248, 21);
			this.tb_content.TabIndex = 2;
			// 
			// tb_Repl
			// 
			this.tb_Repl.Location = new System.Drawing.Point(108, 48);
			this.tb_Repl.Name = "tb_Repl";
			this.tb_Repl.Size = new System.Drawing.Size(248, 21);
			this.tb_Repl.TabIndex = 3;
			// 
			// btfind2
			// 
			this.btfind2.Enabled = false;
			this.btfind2.Location = new System.Drawing.Point(392, 17);
			this.btfind2.Name = "btfind2";
			this.btfind2.Size = new System.Drawing.Size(93, 23);
			this.btfind2.TabIndex = 5;
			this.btfind2.Text = "查找下一个";
			this.btfind2.UseVisualStyleBackColor = true;
			// 
			// bt_Repl
			// 
			this.bt_Repl.Enabled = false;
			this.bt_Repl.Location = new System.Drawing.Point(392, 52);
			this.bt_Repl.Name = "bt_Repl";
			this.bt_Repl.Size = new System.Drawing.Size(93, 23);
			this.bt_Repl.TabIndex = 6;
			this.bt_Repl.Text = "替换";
			this.bt_Repl.UseVisualStyleBackColor = true;
			// 
			// bt_Whole
			// 
			this.bt_Whole.Enabled = false;
			this.bt_Whole.Location = new System.Drawing.Point(393, 84);
			this.bt_Whole.Name = "bt_Whole";
			this.bt_Whole.Size = new System.Drawing.Size(92, 23);
			this.bt_Whole.TabIndex = 7;
			this.bt_Whole.Text = "全部替换";
			this.bt_Whole.UseVisualStyleBackColor = true;
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.Location = new System.Drawing.Point(393, 120);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(92, 23);
			this.bt_Cancel.TabIndex = 8;
			this.bt_Cancel.Text = "取消";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			// 
			// cb_Size
			// 
			this.cb_Size.AutoSize = true;
			this.cb_Size.Location = new System.Drawing.Point(38, 120);
			this.cb_Size.Name = "cb_Size";
			this.cb_Size.Size = new System.Drawing.Size(84, 16);
			this.cb_Size.TabIndex = 9;
			this.cb_Size.Text = "区分大小写";
			this.cb_Size.UseVisualStyleBackColor = true;
			// 
			// Noperepl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(563, 177);
			this.Controls.Add(this.cb_Size);
			this.Controls.Add(this.bt_Cancel);
			this.Controls.Add(this.bt_Whole);
			this.Controls.Add(this.bt_Repl);
			this.Controls.Add(this.btfind2);
			this.Controls.Add(this.tb_Repl);
			this.Controls.Add(this.tb_content);
			this.Controls.Add(this.linkLabel2);
			this.Controls.Add(this.linkLabel1);
			this.Name = "Noperepl";
			this.Text = "Noperepl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.TextBox tb_content;
		private System.Windows.Forms.TextBox tb_Repl;
		private System.Windows.Forms.Button btfind2;
		private System.Windows.Forms.Button bt_Repl;
		private System.Windows.Forms.Button bt_Whole;
		private System.Windows.Forms.Button bt_Cancel;
		private System.Windows.Forms.CheckBox cb_Size;
	}
}