using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;

namespace Nopedemo2
{
	public partial class Nope : Form
	{
		fmFind FmFind = null;  //初始化框，查找框
		String FindString = "";
		bool isUppeN = false;
		bool isDownN = true;
		private bool isSaved = true; //是否保持标示符，true表示已保存。false=未保存
		private string curenFilepath;  //当前保存路径初始为null
		

		#region  字段 全局变量
		log4net.ILog log = log4net.LogManager.GetLogger("Nopedemo2");
		#endregion
		public Nope()
		{
			InitializeComponent();
			log.Info("开始0x1---------------------------------------------------------------------------");
			tsmfind.Click += Tsmfind_Click; //查找
			tsmfind2.Click += Tsmfind2_Click;
			rtxtNotead.KeyUp += RtxtNotead_KeyUp;
			rtxtNotead.MouseDown += RtxtNotead_MouseDown;  //s鼠标事件
			tsmreplace.Click += Tsmreplace_Click;


		}

		private void Tsmreplace_Click(object sender, EventArgs e)
		{
			log.Info("替换");
			Noperepl npRepl = new Noperepl();
			npRepl.Show();
		}

		private void RtxtNotead_MouseDown(object sender, MouseEventArgs e)
		{
			
			log.Info("鼠标事件");
			columnRow();
		}

		private void RtxtNotead_KeyUp(object sender, KeyEventArgs e)
		{
			//键盘时间获取行列
			log.Info("键盘事件");
			columnRow();
			
		}
		private void columnRow() {
			/*得到总行数。该行数会随着文本框的大小改变而改变；若只认回车符为一行(不考虑排版变化)
			请用 总行数=textBox1.Lines.Length;(记事本2是这种方式)
			*/
			int ttalline = rtxtNotead.GetLineFromCharIndex(rtxtNotead.Text.Length) + 1;
			log.Info("ttallin总行数=" + ttalline);
			int index = rtxtNotead.GetFirstCharIndexOfCurrentLine();   //得到当前行第一个字符的索引
			log.Info("当前行第一个字符的索引" + index);
			int line = rtxtNotead.GetLineFromCharIndex(index) + 1;  //得到当前行的行号,从0开始，习惯是从1开始，所以+1.
			log.Info("line得到当前行的行号从始" + line);
			String Line = Convert.ToString(line);
			lCursor.Text = Line;  //多少列
			int col = rtxtNotead.SelectionStart - index;  //SelectionStart得到光标所在位置的索引 减去 当前行第一个字符的索引 = 光标所在的列数
			log.Info("col数==" + col);   //	获取的列数
			String Col = Convert.ToString(col);
			this.lCount.Text = Col;  //只能获取到键盘实际


		}

		

		private void Tsmfind2_Click(object sender, EventArgs e)
		{
			//查找下一个
			serch();
		}

		private void Tsmfind_Click(object sender, EventArgs e)
		{
			log.Info("Tsmlookup_Clic,find----查找=" + FmFind);
			if (this.FmFind == null)
			{

				this.FmFind = new fmFind();  //对象实例化find
				log.Info("FmFind==null=" + FmFind);
				this.FmFind.isAllowClose = false;
				this.FmFind.FmNote = this;
				this.FmFind.TopMost = true; //顶层
			}
			log.Info("跳转查找" + FmFind);
			this.FmFind.Show();
		}

		private void tsmiNew_Click(object sender, EventArgs e)
		{  //新建
			bool isContinue = true;  //保存是否，true，新建保存

			if (!isSaved)
			{

				//说明文本有内容，提示是否保持
				DialogResult result = MessageBox.Show("文件内容被修改，是否保持？", "保存提示",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (result)
				{
					//选择
					case DialogResult.Cancel:
						isContinue = false;
						break;
					case DialogResult.Yes:
						tsmiSave_Click(sender, e);
						//写保存文档代码
						break;

				}


			}
			if (isContinue)//  表示当前文件以保存
			{
				rtxtNotead.Text = string.Empty;  //清空
				this.Text = "新建文本文档-记事本";
				curenFilepath = null;
			}

		}
		internal void serch(String st_Content, String st_Rel, bool bo_CbSiz)
		{
			String st_con = st_Content;
			log.Info("替换查找==" + st_con);
			String st_rel = st_Rel;
			bool bo_cbsiz = bo_CbSiz;
			int start2 = rtxtNotead.Text.IndexOf(st_con,rtxtNotead.SelectionStart);
			log.Info("获取第一个匹配字符的索引==="+start2);
			if (start2==rtxtNotead.SelectionStart&&st_con==rtxtNotead.Text.Substring(rtxtNotead.SelectionStart,rtxtNotead.SelectionLength))
			{
				start2 = rtxtNotead.Text.IndexOf(st_con,rtxtNotead.SelectionStart+1);
				log.Info("start2+1==="+start2);
			}
			if (start2 < 0)
			{
				log.Info("<0" + start2);
				MessageBox.Show("没有找到", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else {
				log.Info(" serch()-找到");
				rtxtNotead.SelectionStart = start2;
				rtxtNotead.SelectionLength = st_con.Length;
				rtxtNotead.ScrollToCaret();
				log.Info(" serch()-找到=" + start2);
			}
		}
		internal void serch(string findString, bool isUper, bool isDown)
		{
			log.Info("serce()=string=" + findString + "bool=" + isUper + " bool=" + isDown);
			FindString = findString;
			log.Info("查找=="+FindString);
			isUppeN = isUper;
			isDownN = isDown;
			serch();
		}
		

		private void serch()
		{
			log.Info("serch()");
			//获取鼠标的位置
			log.Info("查找的n内容=="+FindString);
			int start = rtxtNotead.Text.IndexOf(FindString, rtxtNotead.SelectionStart);
			log.Info("int start==" + start);
			if (start == rtxtNotead.SelectionStart && FindString == rtxtNotead.Text.Substring(rtxtNotead.SelectionStart, rtxtNotead.SelectionLength))
			{
				start = rtxtNotead.Text.IndexOf(FindString, rtxtNotead.SelectionStart + 1);
				log.Info("strt+1=" + start);
			}
			if (start < 0)
			{
				log.Info("start<0,没有找到");
				MessageBox.Show("没找到！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				log.Info(" serch()-找到");
				rtxtNotead.SelectionStart = start;
				rtxtNotead.SelectionLength = FindString.Length;
				rtxtNotead.ScrollToCaret();
				log.Info(" serch()-找到=" + start);
			}

		}

		private void tsmiOpen_Click(object sender, EventArgs e)
		{//打开
			bool isContinue = true;  //true，新建，

			if (!isSaved)
			{
				//说明文本有内容，提示是否保持
				DialogResult result = MessageBox.Show("文件内容被修改，是否保持？", "保存提示",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (result)
				{


					case DialogResult.Cancel:
						isContinue = false;
						break;
					case DialogResult.Yes:
						//写保存文档代码
						tsmiSave_Click(sender, e);
						break;
				}


			}
			if (isContinue)
			{
				OpenFileDialog openFileDilog = new OpenFileDialog();
				openFileDilog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";

				openFileDilog.Title = "打开文件";
				//打开文件，点击OK后就是开始读取文件
				if (openFileDilog.ShowDialog() == DialogResult.OK)
				{
					curenFilepath = openFileDilog.FileName;  // 获取打开的文件路径
															 //读取，设置编码
					StreamReader reader = new StreamReader(openFileDilog.FileName, Encoding.Default);
					rtxtNotead.Text = reader.ReadToEnd();  //读取所有文件
					reader.Close();//关流
								   //读取文件标题
					this.Text = Path.GetFileNameWithoutExtension(openFileDilog.FileName) + "记事本";
				}

			}
		}

		private void tsmiSave_Click(object sender, EventArgs e)
		{//保存
			savaPath();

		}
		//保存函数
		private string GetFilePath()
		{
			//清空文本
			string filePath = string.Empty;
			SaveFileDialog savaFile = new SaveFileDialog();
			savaFile.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
			savaFile.Title = "保存文件";
			if (savaFile.ShowDialog() == DialogResult.OK)
			{
				filePath = savaFile.FileName;
			}
			return filePath;
		}

		private void tsmiFie_Click(object sender, EventArgs e)
		{
			//文件

		}
		//另存为保持函数
		private void savaPath()
		{
			if (curenFilepath == null)
			{
				curenFilepath = GetFilePath();
			}
			//判断路径，写入路径
			if (curenFilepath.Length > 0)
			{
				StreamWriter writer = new StreamWriter(curenFilepath, false, Encoding.Unicode);
				writer.Write(rtxtNotead.Text);
				writer.Close();
				isSaved = true;
			}
			else
			{
				//路径为空
				curenFilepath = null;
			}

		}


		private void rtxtNotead_TextChanged(object sender, EventArgs e)
		{

			//多格式文本框      文本被修改，表示未保存
			isSaved = false;

		}



		private void Form1_Load(object sender, EventArgs e)
		{
			//外大框
		}

		private void tsmiClose_Click(object sender, EventArgs e)
		{

			//退出
			Application.Exit();  // 程序退出
		}

		private void tsmiEdit_Click(object sender, EventArgs e)
		{
			//编辑
		}



		private void tsmiSeAs_Click(object sender, EventArgs e)
		{        //另存为	

			savaPath();//保持
		}

		private void tsmiUndo_Click(object sender, EventArgs e)
		{
			//撤销
			rtxtNotead.Undo();
		}

		private void tsmiCut_Click(object sender, EventArgs e)
		{
			//剪切
			rtxtNotead.Cut();

		}

		private void tsmiCopy_Click(object sender, EventArgs e)
		{
			//复制
			rtxtNotead.Copy();
		}

		private void tsmiPaste_Click(object sender, EventArgs e)
		{
			//粘贴
			rtxtNotead.Paste();
		}

		private void tsmiSelectAll_Click(object sender, EventArgs e)
		{
			//全选
			rtxtNotead.SelectAll();
		}

		private void tsmiDate_Click(object sender, EventArgs e)
		{
			//时间日期
			rtxtNotead.AppendText(System.DateTime.Now.ToString());
		}

		private void tsmiAbout_Click(object sender, EventArgs e)
		{
			//关于
			//MessageBox.Show
			string stb = "版本: 2.2.01    \n版权所有: @2017-2099 粟敏保留所有权。   \n本产品使用权: \nhttp://www.cnblogs.com/hs22/";
			MessageBox.Show(stb);
		}

		private void tsmiHelpp_Click(object sender, EventArgs e)
		{
			string st = "仅供学习，\n严禁商业用途。\n";
			MessageBox.Show(st);
		}


	}
}
