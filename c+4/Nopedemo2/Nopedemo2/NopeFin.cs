using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nopedemo2
{
	public partial class fmFind : Form
	{
		log4net.ILog log = log4net.LogManager.GetLogger("fmFind");
		public bool isAllowClose = true;   //find窗体true 表示存在

		public Nope FmNote { get; internal set; }    //主窗体定义

		public String FindString = "";  //获取查找字符

		bool isUper = false;  //是否勾选区分大小写，默认是false未勾选。
		bool isDown = true;  //是否勾选向上向下，true

		public fmFind()
		{
			InitializeComponent();
			bfind.Click += Bfind_Click;     //	 查找
			bcancel.Click += Bcancel_Click;     //	取消
			FormClosing += FmFind_FormClosing;  //窗体关闭实践
		}

		private void FmFind_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!isAllowClose)
			{
				e.Cancel = true;
				Hide();  //隐藏UI
			}
		}

		private void Bcancel_Click(object sender, EventArgs e)
		{
			log.Info("Bcancel_Click---quexiao");
			this.isAllowClose = true;
			DialogResult = DialogResult.Cancel;
			//this.DialogResult = DialogResult.Cancel;  //调用取消

		}

		private void Bfind_Click(object sender, EventArgs e)
		{
			log.Info("Bfind_Click");
			FindString = this.tfindBox.Text;  //获取查找的内容
			isUper = rdoUP.Checked; //获取选中向上
			isDown = rdoDown.Checked; //获取空间空间是否选中向下
			log.Info("获取的查找内容find=" + FindString);
			if (FindString != "")
			{
				log.Info("FindString!=null");
				DialogResult = DialogResult.OK;

			}
			if (!isAllowClose)
			{

				log.Info("!isAllowClose=" + isAllowClose);
				FmNote.serch(FindString, isUper, isDown);   //调用nope方法
			}
		}


	}
}
