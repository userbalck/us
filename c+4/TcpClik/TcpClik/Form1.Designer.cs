namespace TcpClik
{
	partial class Form1
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
			this.StartClik = new System.Windows.Forms.Button();
			this.StopClik = new System.Windows.Forms.Button();
			this.textclik = new System.Windows.Forms.TextBox();
			this.Sendclik = new System.Windows.Forms.Button();
			this.tblog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// StartClik
			// 
			this.StartClik.Location = new System.Drawing.Point(12, 12);
			this.StartClik.Name = "StartClik";
			this.StartClik.Size = new System.Drawing.Size(75, 23);
			this.StartClik.TabIndex = 0;
			this.StartClik.Text = "启动客户端";
			this.StartClik.UseVisualStyleBackColor = true;
			// 
			// StopClik
			// 
			this.StopClik.Location = new System.Drawing.Point(123, 11);
			this.StopClik.Name = "StopClik";
			this.StopClik.Size = new System.Drawing.Size(75, 23);
			this.StopClik.TabIndex = 1;
			this.StopClik.Text = "关闭客户端";
			this.StopClik.UseVisualStyleBackColor = true;
			// 
			// textclik
			// 
			this.textclik.Location = new System.Drawing.Point(3, 51);
			this.textclik.Multiline = true;
			this.textclik.Name = "textclik";
			this.textclik.Size = new System.Drawing.Size(213, 305);
			this.textclik.TabIndex = 2;
			// 
			// Sendclik
			// 
			this.Sendclik.Location = new System.Drawing.Point(222, 51);
			this.Sendclik.Name = "Sendclik";
			this.Sendclik.Size = new System.Drawing.Size(75, 23);
			this.Sendclik.TabIndex = 3;
			this.Sendclik.Text = "发送";
			this.Sendclik.UseVisualStyleBackColor = true;
			// 
			// tblog
			// 
			this.tblog.Location = new System.Drawing.Point(353, 51);
			this.tblog.Multiline = true;
			this.tblog.Name = "tblog";
			this.tblog.Size = new System.Drawing.Size(209, 298);
			this.tblog.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(574, 361);
			this.Controls.Add(this.tblog);
			this.Controls.Add(this.Sendclik);
			this.Controls.Add(this.textclik);
			this.Controls.Add(this.StopClik);
			this.Controls.Add(this.StartClik);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button StartClik;
		private System.Windows.Forms.Button StopClik;
		private System.Windows.Forms.TextBox textclik;
		private System.Windows.Forms.Button Sendclik;
		private System.Windows.Forms.TextBox tblog;
	}
}

