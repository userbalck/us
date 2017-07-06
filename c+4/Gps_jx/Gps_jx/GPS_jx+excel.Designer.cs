namespace Gps_jx
{
	partial class GPS_jx_excel
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.text_jgs = new System.Windows.Forms.TextBox();
			this.pb_Gps = new System.Windows.Forms.ProgressBar();
			this.menuStrip2 = new System.Windows.Forms.MenuStrip();
			this.open_gps = new System.Windows.Forms.ToolStripMenuItem();
			this.jx_sava = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip2.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Location = new System.Drawing.Point(0, 25);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(583, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// text_jgs
			// 
			this.text_jgs.Location = new System.Drawing.Point(0, 27);
			this.text_jgs.Multiline = true;
			this.text_jgs.Name = "text_jgs";
			this.text_jgs.Size = new System.Drawing.Size(583, 267);
			this.text_jgs.TabIndex = 1;
			// 
			// pb_Gps
			// 
			this.pb_Gps.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pb_Gps.Location = new System.Drawing.Point(0, 301);
			this.pb_Gps.Name = "pb_Gps";
			this.pb_Gps.Size = new System.Drawing.Size(583, 23);
			this.pb_Gps.TabIndex = 2;
			this.pb_Gps.Value = 55;
			// 
			// menuStrip2
			// 
			this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open_gps,
            this.jx_sava});
			this.menuStrip2.Location = new System.Drawing.Point(0, 0);
			this.menuStrip2.Name = "menuStrip2";
			this.menuStrip2.Size = new System.Drawing.Size(583, 25);
			this.menuStrip2.TabIndex = 3;
			this.menuStrip2.Text = "menuStrip2";
			// 
			// open_gps
			// 
			this.open_gps.Name = "open_gps";
			this.open_gps.Size = new System.Drawing.Size(44, 21);
			this.open_gps.Text = "打开";
			// 
			// jx_sava
			// 
			this.jx_sava.Name = "jx_sava";
			this.jx_sava.Size = new System.Drawing.Size(68, 21);
			this.jx_sava.Text = "解析存储";
			// 
			// GPS_jx_excel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(583, 324);
			this.Controls.Add(this.pb_Gps);
			this.Controls.Add(this.text_jgs);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.menuStrip2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "GPS_jx_excel";
			this.Text = "GPS_jx_excel";
			this.menuStrip2.ResumeLayout(false);
			this.menuStrip2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.TextBox text_jgs;
		private System.Windows.Forms.ProgressBar pb_Gps;
		private System.Windows.Forms.MenuStrip menuStrip2;
		private System.Windows.Forms.ToolStripMenuItem open_gps;
		private System.Windows.Forms.ToolStripMenuItem jx_sava;
	}
}