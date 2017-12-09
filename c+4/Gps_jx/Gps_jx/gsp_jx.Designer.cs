namespace Gps_jx
{
	partial class gsp_jx
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.psb_Gps = new System.Windows.Forms.ProgressBar();
			this.Gps_text = new System.Windows.Forms.TextBox();
			this.menuStrip2 = new System.Windows.Forms.MenuStrip();
			this.open_Gps = new System.Windows.Forms.ToolStripMenuItem();
			this.sava_Gps = new System.Windows.Forms.ToolStripMenuItem();
			this.NewGps = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip2.SuspendLayout();
			this.SuspendLayout();
			// 
			// psb_Gps
			// 
			this.psb_Gps.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.psb_Gps.Location = new System.Drawing.Point(0, 331);
			this.psb_Gps.Name = "psb_Gps";
			this.psb_Gps.Size = new System.Drawing.Size(662, 23);
			this.psb_Gps.TabIndex = 0;
			this.psb_Gps.Value = 55;
			// 
			// Gps_text
			// 
			this.Gps_text.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Gps_text.Location = new System.Drawing.Point(0, 25);
			this.Gps_text.Multiline = true;
			this.Gps_text.Name = "Gps_text";
			this.Gps_text.Size = new System.Drawing.Size(662, 306);
			this.Gps_text.TabIndex = 1;
			// 
			// menuStrip2
			// 
			this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open_Gps,
            this.sava_Gps,
            this.NewGps});
			this.menuStrip2.Location = new System.Drawing.Point(0, 0);
			this.menuStrip2.Name = "menuStrip2";
			this.menuStrip2.Size = new System.Drawing.Size(662, 25);
			this.menuStrip2.TabIndex = 3;
			this.menuStrip2.Text = "menuStrip2";
			// 
			// open_Gps
			// 
			this.open_Gps.Name = "open_Gps";
			this.open_Gps.Size = new System.Drawing.Size(44, 21);
			this.open_Gps.Text = "打开";
			// 
			// sava_Gps
			// 
			this.sava_Gps.Name = "sava_Gps";
			this.sava_Gps.Size = new System.Drawing.Size(44, 21);
			this.sava_Gps.Text = "保存";
			// 
			// NewGps
			// 
			this.NewGps.Name = "NewGps";
			this.NewGps.Size = new System.Drawing.Size(44, 21);
			this.NewGps.Text = "新建";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(662, 354);
			this.Controls.Add(this.Gps_text);
			this.Controls.Add(this.psb_Gps);
			this.Controls.Add(this.menuStrip2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.menuStrip2.ResumeLayout(false);
			this.menuStrip2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar psb_Gps;
		private System.Windows.Forms.TextBox Gps_text;
		private System.Windows.Forms.MenuStrip menuStrip2;
		private System.Windows.Forms.ToolStripMenuItem open_Gps;
		private System.Windows.Forms.ToolStripMenuItem sava_Gps;
		private System.Windows.Forms.ToolStripMenuItem NewGps;
	}
}

