namespace Nopedemo2
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnusNotepad = new System.Windows.Forms.MenuStrip();
			this.tsmiFie = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSeAs = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmlookup = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiDate = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiFormat = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAuto = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiFont = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiToolStri = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiHelpp = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.rtxtNotead = new System.Windows.Forms.RichTextBox();
			this.odlgNotepad = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.fdlgNotepad = new System.Windows.Forms.FontDialog();
			this.tmrNotepad = new System.Windows.Forms.Timer(this.components);
			this.mnusNotepad.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// mnusNotepad
			// 
			this.mnusNotepad.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFie,
            this.tsmiEdit,
            this.tsmiFormat,
            this.tsmiView,
            this.tsmiHelp});
			this.mnusNotepad.Location = new System.Drawing.Point(0, 0);
			this.mnusNotepad.Name = "mnusNotepad";
			this.mnusNotepad.Size = new System.Drawing.Size(586, 25);
			this.mnusNotepad.TabIndex = 1;
			this.mnusNotepad.Text = "menuStrip1";
			// 
			// tsmiFie
			// 
			this.tsmiFie.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiOpen,
            this.tsmiSave,
            this.tsmiSeAs,
            this.tsmiClose});
			this.tsmiFie.Name = "tsmiFie";
			this.tsmiFie.Size = new System.Drawing.Size(44, 21);
			this.tsmiFie.Text = "文件";
			this.tsmiFie.Click += new System.EventHandler(this.tsmiFie_Click);
			// 
			// tsmiNew
			// 
			this.tsmiNew.Name = "tsmiNew";
			this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.tsmiNew.Size = new System.Drawing.Size(152, 22);
			this.tsmiNew.Text = "新建";
			this.tsmiNew.Click += new System.EventHandler(this.tsmiNew_Click);
			// 
			// tsmiOpen
			// 
			this.tsmiOpen.Name = "tsmiOpen";
			this.tsmiOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.tsmiOpen.Size = new System.Drawing.Size(152, 22);
			this.tsmiOpen.Text = "打开";
			this.tsmiOpen.Click += new System.EventHandler(this.tsmiOpen_Click);
			// 
			// tsmiSave
			// 
			this.tsmiSave.Name = "tsmiSave";
			this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.tsmiSave.Size = new System.Drawing.Size(152, 22);
			this.tsmiSave.Text = "保存";
			this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
			// 
			// tsmiSeAs
			// 
			this.tsmiSeAs.Name = "tsmiSeAs";
			this.tsmiSeAs.Size = new System.Drawing.Size(152, 22);
			this.tsmiSeAs.Text = "另存为";
			this.tsmiSeAs.Click += new System.EventHandler(this.tsmiSeAs_Click);
			// 
			// tsmiClose
			// 
			this.tsmiClose.Name = "tsmiClose";
			this.tsmiClose.Size = new System.Drawing.Size(152, 22);
			this.tsmiClose.Text = "退出";
			this.tsmiClose.Click += new System.EventHandler(this.tsmiClose_Click);
			// 
			// tsmiEdit
			// 
			this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUndo,
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.tsmlookup,
            this.tsmiSelectAll,
            this.tsmiDate});
			this.tsmiEdit.Name = "tsmiEdit";
			this.tsmiEdit.Size = new System.Drawing.Size(44, 21);
			this.tsmiEdit.Text = "编辑";
			this.tsmiEdit.Click += new System.EventHandler(this.tsmiEdit_Click);
			// 
			// tsmiUndo
			// 
			this.tsmiUndo.Name = "tsmiUndo";
			this.tsmiUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.tsmiUndo.Size = new System.Drawing.Size(152, 22);
			this.tsmiUndo.Text = "撤销";
			this.tsmiUndo.Click += new System.EventHandler(this.tsmiUndo_Click);
			// 
			// tsmiCut
			// 
			this.tsmiCut.Name = "tsmiCut";
			this.tsmiCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.tsmiCut.Size = new System.Drawing.Size(152, 22);
			this.tsmiCut.Text = "剪切";
			this.tsmiCut.Click += new System.EventHandler(this.tsmiCut_Click);
			// 
			// tsmiCopy
			// 
			this.tsmiCopy.Name = "tsmiCopy";
			this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.tsmiCopy.Size = new System.Drawing.Size(152, 22);
			this.tsmiCopy.Text = "复制";
			this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
			// 
			// tsmiPaste
			// 
			this.tsmiPaste.Name = "tsmiPaste";
			this.tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.tsmiPaste.Size = new System.Drawing.Size(152, 22);
			this.tsmiPaste.Text = "粘贴";
			this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
			// 
			// tsmlookup
			// 
			this.tsmlookup.Name = "tsmlookup";
			this.tsmlookup.Size = new System.Drawing.Size(152, 22);
			this.tsmlookup.Text = "查找";
			this.tsmlookup.Click += new System.EventHandler(this.tsmlookup_Click);
			// 
			// tsmiSelectAll
			// 
			this.tsmiSelectAll.Name = "tsmiSelectAll";
			this.tsmiSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.tsmiSelectAll.Size = new System.Drawing.Size(152, 22);
			this.tsmiSelectAll.Text = "全选";
			this.tsmiSelectAll.Click += new System.EventHandler(this.tsmiSelectAll_Click);
			// 
			// tsmiDate
			// 
			this.tsmiDate.Name = "tsmiDate";
			this.tsmiDate.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.tsmiDate.Size = new System.Drawing.Size(152, 22);
			this.tsmiDate.Text = "时间/日期";
			this.tsmiDate.Click += new System.EventHandler(this.tsmiDate_Click);
			// 
			// tsmiFormat
			// 
			this.tsmiFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAuto,
            this.tsmiFont});
			this.tsmiFormat.Name = "tsmiFormat";
			this.tsmiFormat.Size = new System.Drawing.Size(44, 21);
			this.tsmiFormat.Text = "格式";
			// 
			// tsmiAuto
			// 
			this.tsmiAuto.Name = "tsmiAuto";
			this.tsmiAuto.Size = new System.Drawing.Size(152, 22);
			this.tsmiAuto.Text = "自动换行";
			// 
			// tsmiFont
			// 
			this.tsmiFont.Name = "tsmiFont";
			this.tsmiFont.Size = new System.Drawing.Size(152, 22);
			this.tsmiFont.Text = "字体";
			// 
			// tsmiView
			// 
			this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiToolStri});
			this.tsmiView.Name = "tsmiView";
			this.tsmiView.Size = new System.Drawing.Size(44, 21);
			this.tsmiView.Text = "查看";
			// 
			// tsmiToolStri
			// 
			this.tsmiToolStri.Name = "tsmiToolStri";
			this.tsmiToolStri.Size = new System.Drawing.Size(152, 22);
			this.tsmiToolStri.Text = "工具栏";
			// 
			// tsmiHelp
			// 
			this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiHelpp,
            this.tsmiAbout});
			this.tsmiHelp.Name = "tsmiHelp";
			this.tsmiHelp.Size = new System.Drawing.Size(44, 21);
			this.tsmiHelp.Text = "帮助";
			// 
			// tsmiHelpp
			// 
			this.tsmiHelpp.Name = "tsmiHelpp";
			this.tsmiHelpp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.tsmiHelpp.Size = new System.Drawing.Size(179, 22);
			this.tsmiHelpp.Text = "查看帮助";
			this.tsmiHelpp.Click += new System.EventHandler(this.tsmiHelpp_Click);
			// 
			// tsmiAbout
			// 
			this.tsmiAbout.Name = "tsmiAbout";
			this.tsmiAbout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.tsmiAbout.Size = new System.Drawing.Size(179, 22);
			this.tsmiAbout.Text = "关于记事本";
			this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
			// 
			// rtxtNotead
			// 
			this.rtxtNotead.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtxtNotead.Location = new System.Drawing.Point(0, 28);
			this.rtxtNotead.Name = "rtxtNotead";
			this.rtxtNotead.Size = new System.Drawing.Size(586, 233);
			this.rtxtNotead.TabIndex = 2;
			this.rtxtNotead.Text = "";
			this.rtxtNotead.TextChanged += new System.EventHandler(this.rtxtNotead_TextChanged);
			// 
			// odlgNotepad
			// 
			this.odlgNotepad.FileName = "openFileDialog2";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 262);
			this.Controls.Add(this.rtxtNotead);
			this.Controls.Add(this.mnusNotepad);
			this.MainMenuStrip = this.mnusNotepad;
			this.Name = "Form1";
			this.Text = "记事本";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.mnusNotepad.ResumeLayout(false);
			this.mnusNotepad.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.MenuStrip mnusNotepad;
        private System.Windows.Forms.ToolStripMenuItem tsmiFie;
        private System.Windows.Forms.ToolStripMenuItem tsmiNew;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.RichTextBox rtxtNotead;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.OpenFileDialog odlgNotepad;
        private System.Windows.Forms.ToolStripMenuItem tsmiClose;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiUndo;
        private System.Windows.Forms.ToolStripMenuItem tsmiFormat;
        private System.Windows.Forms.ToolStripMenuItem tsmiCut;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmlookup;
        private System.Windows.Forms.ToolStripMenuItem tsmiSeAs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem tsmiAuto;
        private System.Windows.Forms.ToolStripMenuItem tsmiFont;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private System.Windows.Forms.ToolStripMenuItem tsmiToolStri;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelpp;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FontDialog fdlgNotepad;
        private System.Windows.Forms.Timer tmrNotepad;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDate;
	}
}

