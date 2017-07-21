namespace test_xml
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
			this.tvXml = new System.Windows.Forms.TreeView();
			this.btCreate = new System.Windows.Forms.Button();
			this.btCllean = new System.Windows.Forms.Button();
			this.btDelet = new System.Windows.Forms.Button();
			this.txlog = new System.Windows.Forms.TextBox();
			this.tbi1 = new System.Windows.Forms.TextBox();
			this.tbi2 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btOpen = new System.Windows.Forms.Button();
			this.butJson = new System.Windows.Forms.Button();
			this.butWr = new System.Windows.Forms.Button();
			this.butSava = new System.Windows.Forms.Button();
			this.btXML = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tvXml
			// 
			this.tvXml.Location = new System.Drawing.Point(1, 1);
			this.tvXml.Name = "tvXml";
			this.tvXml.Size = new System.Drawing.Size(340, 344);
			this.tvXml.TabIndex = 1;
			// 
			// btCreate
			// 
			this.btCreate.Location = new System.Drawing.Point(450, 144);
			this.btCreate.Name = "btCreate";
			this.btCreate.Size = new System.Drawing.Size(75, 23);
			this.btCreate.TabIndex = 2;
			this.btCreate.Text = "生成节点";
			this.btCreate.UseVisualStyleBackColor = true;
			// 
			// btCllean
			// 
			this.btCllean.Location = new System.Drawing.Point(450, 222);
			this.btCllean.Name = "btCllean";
			this.btCllean.Size = new System.Drawing.Size(75, 23);
			this.btCllean.TabIndex = 3;
			this.btCllean.Text = "清除";
			this.btCllean.UseVisualStyleBackColor = true;
			// 
			// btDelet
			// 
			this.btDelet.Location = new System.Drawing.Point(450, 184);
			this.btDelet.Name = "btDelet";
			this.btDelet.Size = new System.Drawing.Size(75, 23);
			this.btDelet.TabIndex = 4;
			this.btDelet.Text = "删除节点";
			this.btDelet.UseVisualStyleBackColor = true;
			// 
			// txlog
			// 
			this.txlog.Location = new System.Drawing.Point(1, 351);
			this.txlog.Multiline = true;
			this.txlog.Name = "txlog";
			this.txlog.Size = new System.Drawing.Size(526, 171);
			this.txlog.TabIndex = 5;
			// 
			// tbi1
			// 
			this.tbi1.Location = new System.Drawing.Point(415, 16);
			this.tbi1.Name = "tbi1";
			this.tbi1.Size = new System.Drawing.Size(70, 21);
			this.tbi1.TabIndex = 6;
			// 
			// tbi2
			// 
			this.tbi2.Location = new System.Drawing.Point(418, 58);
			this.tbi2.Name = "tbi2";
			this.tbi2.Size = new System.Drawing.Size(70, 21);
			this.tbi2.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(347, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "父节点数量";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(347, 61);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 9;
			this.label2.Text = "子节点数量";
			// 
			// btOpen
			// 
			this.btOpen.Location = new System.Drawing.Point(450, 104);
			this.btOpen.Name = "btOpen";
			this.btOpen.Size = new System.Drawing.Size(75, 23);
			this.btOpen.TabIndex = 11;
			this.btOpen.Text = "打开";
			this.btOpen.UseVisualStyleBackColor = true;
			// 
			// butJson
			// 
			this.butJson.Location = new System.Drawing.Point(349, 144);
			this.butJson.Name = "butJson";
			this.butJson.Size = new System.Drawing.Size(75, 23);
			this.butJson.TabIndex = 12;
			this.butJson.Text = "打开JSON";
			this.butJson.UseVisualStyleBackColor = true;
			// 
			// butWr
			// 
			this.butWr.Location = new System.Drawing.Point(349, 184);
			this.butWr.Name = "butWr";
			this.butWr.Size = new System.Drawing.Size(75, 23);
			this.butWr.TabIndex = 13;
			this.butWr.Text = "写json";
			this.butWr.UseVisualStyleBackColor = true;
			// 
			// butSava
			// 
			this.butSava.Location = new System.Drawing.Point(349, 222);
			this.butSava.Name = "butSava";
			this.butSava.Size = new System.Drawing.Size(75, 23);
			this.butSava.TabIndex = 14;
			this.butSava.Text = "保存";
			this.butSava.UseVisualStyleBackColor = true;
			// 
			// btXML
			// 
			this.btXML.Location = new System.Drawing.Point(347, 104);
			this.btXML.Name = "btXML";
			this.btXML.Size = new System.Drawing.Size(75, 23);
			this.btXML.TabIndex = 15;
			this.btXML.Text = "打开XML";
			this.btXML.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(539, 522);
			this.Controls.Add(this.btXML);
			this.Controls.Add(this.butSava);
			this.Controls.Add(this.butWr);
			this.Controls.Add(this.butJson);
			this.Controls.Add(this.btOpen);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbi2);
			this.Controls.Add(this.tbi1);
			this.Controls.Add(this.txlog);
			this.Controls.Add(this.btDelet);
			this.Controls.Add(this.btCllean);
			this.Controls.Add(this.btCreate);
			this.Controls.Add(this.tvXml);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TreeView tvXml;
		private System.Windows.Forms.Button btCreate;
		private System.Windows.Forms.Button btCllean;
		private System.Windows.Forms.Button btDelet;
		private System.Windows.Forms.TextBox txlog;
		private System.Windows.Forms.TextBox tbi1;
		private System.Windows.Forms.TextBox tbi2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btOpen;
		private System.Windows.Forms.Button butJson;
		private System.Windows.Forms.Button butWr;
		private System.Windows.Forms.Button butSava;
		private System.Windows.Forms.Button btXML;
	}
}

